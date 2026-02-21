using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service implementation for inventory management operations
/// </summary>
public class InventoryManagementService(
    IInventoryRepository inventoryRepository,
    IInventoryTransactionRepository transactionRepository,
    IProductCatalogService productCatalogService,
    ILogger<InventoryManagementService> logger)
    : IInventoryManagementService
{
    // Inventory CRUD operations
    public async Task<Inventory> CreateInventoryAsync(CreateInventoryRequest request, CancellationToken cancellationToken = default)
    {
        // Validate request
        var validationResult = await ValidateInventoryRequest(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validationResult.Errors));
        }

        // Check if inventory already exists
        var existingInventory = await inventoryRepository.GetByProductAndLocationAsync(
            request.ProductId,
            request.ProductVariationId,
            request.BranchId,
            request.WarehouseId,
            cancellationToken);

        if (existingInventory != null)
        {
            throw new InvalidOperationException("Inventory already exists for this product and location");
        }

        // Get product information
        var product = await productCatalogService.GetProductAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new ArgumentException("Product not found");
        }

        // Create inventory
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            ProductVariationId = request.ProductVariationId,
            BranchId = request.BranchId,
            WarehouseId = request.WarehouseId,
            Quantity = request.Quantity,
            ReservedQuantity = 0,
            AvailableQuantity = request.Quantity,
            UnitCost = request.UnitCost,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        var createdInventory = await inventoryRepository.AddAsync(inventory, cancellationToken);

        // Create initial transaction
        await CreateTransactionAsync(
            createdInventory.Id,
            "IN",
            request.Quantity,
            request.UnitCost,
            $"INV-{createdInventory.Id:N8}",
            request.Reason,
            request.CreatedByUserId,
            null,
            null,
            null,
            null,
            cancellationToken);

        logger.LogInformation("Inventory created successfully with ID: {InventoryId}, ProductId: {ProductId}, Quantity: {Quantity}", inventory.Id, request.ProductId, request.Quantity);

        return createdInventory;
    }

    public async Task<Inventory?> GetInventoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Inventory> UpdateInventoryAsync(Guid id, UpdateInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory == null)
        {
            throw new KeyNotFoundException($"Inventory with ID {id} not found");
        }

        // Validate request
        var validationResult = await ValidateInventoryRequest(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validationResult.Errors));
        }

        // Update inventory
        inventory.Quantity = request.Quantity;
        inventory.AvailableQuantity = inventory.Quantity - inventory.ReservedQuantity;
        inventory.UnitCost = request.UnitCost;
        inventory.LastUpdated = DateTime.UtcNow;

        await inventoryRepository.UpdateQuantityAsync(id, inventory.Quantity, inventory.UnitCost, request.Reason, request.UpdatedByUserId, cancellationToken);

        // Create adjustment transaction
        var quantityDiff = request.Quantity - inventory.Quantity;
        await CreateTransactionAsync(
            id,
            quantityDiff > 0 ? "IN" : "OUT",
            Math.Abs(quantityDiff),
            request.UnitCost,
            $"INV-{id}:N8",
            request.Reason,
            request.UpdatedByUserId,
            null,
            null,
            null,
            null,
            cancellationToken);

        logger.LogInformation("Inventory updated successfully with ID: {InventoryId}, NewQuantity: {NewQuantity}, OldQuantity: {OldQuantity}", inventory.Id, request.Quantity, inventory.Quantity);

        return inventory;
    }

    public async Task DeleteInventoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory == null)
        {
            throw new KeyNotFoundException($"Inventory with ID {id} not found");
        }

        // Create deletion transaction
        await CreateTransactionAsync(
            id,
            "OUT",
            inventory.Quantity,
            inventory.UnitCost,
            $"INV-{id}:N8",
            "Inventory deleted",
            Guid.Empty, // Will be set to current user
            null,
            null,
            null,
            null,
            cancellationToken);

        await inventoryRepository.DeleteAsync(id, cancellationToken);

        logger.LogInformation("Inventory deleted successfully with ID: {InventoryId}", inventory.Id);
    }

    // Inventory location operations
    public async Task<Inventory?> GetInventoryByLocationAsync(
        Guid productId, 
        Guid? productVariationId, 
        Guid branchId, 
        Guid? warehouseId, 
        CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetByProductAndLocationAsync(
            productId,
            productVariationId,
            branchId,
            warehouseId,
            cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetInventoryByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetByProductAsync(productId, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetInventoryByProductVariationAsync(Guid productVariationId, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetByProductVariationAsync(productVariationId, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetInventoryByBranchAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetByBranchAsync(branchId, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetInventoryByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetByWarehouseAsync(warehouseId, cancellationToken);
    }

    // Stock management operations
    public async Task ReserveInventoryAsync(Guid inventoryId, ReserveInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(inventoryId, cancellationToken);
        if (inventory == null)
        {
            throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
        }

        if (inventory.AvailableQuantity < request.Quantity)
        {
            throw new InvalidOperationException($"Insufficient available quantity. Available: {inventory.AvailableQuantity}, Requested: {request.Quantity}");
        }

        // Reserve inventory
        await inventoryRepository.ReserveQuantityAsync(inventoryId, request.Quantity, cancellationToken);

        // Create reservation transaction
        await CreateTransactionAsync(
            inventoryId,
            "RESERVATION",
            request.Quantity,
            inventory.UnitCost,
            $"RES-{inventoryId:N8}",
            request.Reason,
            request.ReservedByUserId,
            null,
            null,
            null,
            null,
            cancellationToken);

        logger.LogInformation("Inventory reserved successfully with ID: {InventoryId}, Quantity: {Quantity}", inventoryId, request.Quantity);
    }

    public async Task ReleaseInventoryAsync(Guid inventoryId, ReleaseInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(inventoryId, cancellationToken);
        if (inventory == null)
        {
            throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
        }

        if (inventory.ReservedQuantity < request.Quantity)
        {
            throw new InvalidOperationException($"Cannot release more than reserved quantity. Reserved: {inventory.ReservedQuantity}, Requested: {request.Quantity}");
        }

        // Release inventory
        await inventoryRepository.ReleaseReservedQuantityAsync(inventoryId, request.Quantity, cancellationToken);

        // Create release transaction
        await CreateTransactionAsync(
            inventoryId,
            "RELEASE",
            request.Quantity,
            inventory.UnitCost,
            $"REL-{inventoryId:N8}",
            request.Reason,
            request.ReleasedByUserId,
            null,
            null,
            null,
            null,
            cancellationToken);

        logger.LogInformation("Inventory released successfully with ID: {InventoryId}, Quantity: {Quantity}", inventoryId, request.Quantity);
    }

    public async Task<Inventory> UpdateStockAsync(Guid id, int quantity, decimal unitCost, string reason, Guid userId, CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory == null)
        {
            throw new KeyNotFoundException($"Inventory with ID {id} not found");
        }

        var oldQuantity = inventory.Quantity;
        inventory.Quantity = quantity;
        inventory.AvailableQuantity = quantity - inventory.ReservedQuantity;
        inventory.UnitCost = unitCost;
        inventory.LastUpdated = DateTime.UtcNow;

        await inventoryRepository.UpdateQuantityAsync(id, quantity, unitCost, reason, userId, cancellationToken);

        // Create stock update transaction
        var quantityDiff = quantity - oldQuantity;
        await CreateTransactionAsync(
            id,
            quantityDiff > 0 ? "IN" : "OUT",
            Math.Abs(quantityDiff),
            unitCost,
            $"STK-{id:N8}",
            reason,
            userId,
            null,
            null,
            null,
            null,
            cancellationToken);

        logger.LogInformation("Stock updated successfully with ID: {InventoryId}, OldQuantity: {OldQuantity}, NewQuantity: {NewQuantity}", id, oldQuantity, quantity);

        return inventory;
    }

    // Transfer operations
    public async Task<InventoryTransferResult> TransferInventoryAsync(TransferInventoryRequest request, CancellationToken cancellationToken = default)
    {
        // Validate request
        var validationResult = await ValidateTransferRequest(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new InventoryTransferResult
            {
                Success = false,
                FromInventoryId = request.FromInventoryId,
                ToInventoryId = Guid.Empty,
                TransferredQuantity = 0,
                TransferredValue = 0,
                Message = string.Join(", ", validationResult.Errors)
            };
        }

        // Get source inventory
        var fromInventory = await inventoryRepository.GetByIdAsync(request.FromInventoryId, cancellationToken);
        if (fromInventory == null)
        {
            return new InventoryTransferResult
            {
                Success = false,
                FromInventoryId = request.FromInventoryId,
                ToInventoryId = Guid.Empty,
                TransferredQuantity = 0,
                TransferredValue = 0,
                Message = "Source inventory not found"
            };
        }

        // Check if sufficient quantity available
        if (fromInventory.AvailableQuantity < request.Quantity)
        {
            return new InventoryTransferResult
            {
                Success = false,
                FromInventoryId = request.FromInventoryId,
                ToInventoryId = Guid.Empty,
                TransferredQuantity = 0,
                TransferredValue = 0,
                Message = $"Insufficient quantity available. Available: {fromInventory.AvailableQuantity}, Requested: {request.Quantity}"
            };
        }

        // Get or create destination inventory
        var toInventory = await inventoryRepository.GetByProductAndLocationAsync(
            fromInventory.ProductId,
            fromInventory.ProductVariationId,
            request.ToBranchId,
            request.ToWarehouseId,
            cancellationToken);

        if (toInventory == null)
        {
            // Create destination inventory
            var product = await productCatalogService.GetProductAsync(fromInventory.ProductId, cancellationToken);
            if (product == null)
            {
                return new InventoryTransferResult
                {
                    Success = false,
                    FromInventoryId = request.FromInventoryId,
                    ToInventoryId = Guid.Empty,
                    TransferredQuantity = 0,
                    TransferredValue = 0,
                    Message = "Product not found for destination inventory creation"
                };
            }

            toInventory = new Inventory
            {
                Id = Guid.NewGuid(),
                ProductId = fromInventory.ProductId,
                ProductVariationId = fromInventory.ProductVariationId,
                BranchId = request.ToBranchId,
                WarehouseId = request.ToWarehouseId,
                Quantity = 0,
                ReservedQuantity = 0,
                AvailableQuantity = 0,
                UnitCost = request.UnitCost,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            toInventory = await inventoryRepository.AddAsync(toInventory, cancellationToken);
        }

        // Perform transfer
        fromInventory.Quantity -= request.Quantity;
        fromInventory.AvailableQuantity -= request.Quantity;
        fromInventory.LastUpdated = DateTime.UtcNow;

        toInventory.Quantity += request.Quantity;
        toInventory.AvailableQuantity += request.Quantity;
        toInventory.LastUpdated = DateTime.UtcNow;

        await inventoryRepository.UpdateQuantityAsync(fromInventory.Id, fromInventory.Quantity, fromInventory.UnitCost, "Transfer out", request.TransferredByUserId, cancellationToken);
        await inventoryRepository.UpdateQuantityAsync(toInventory.Id, toInventory.Quantity, toInventory.UnitCost, "Transfer in", request.TransferredByUserId, cancellationToken);

        // Create transfer transactions
        var transferOutTransaction = await CreateTransactionAsync(
            fromInventory.Id,
            "TRANSFER",
            request.Quantity,
            request.UnitCost,
            $"TRF-{fromInventory.Id}:N8",
            request.Reason,
            request.TransferredByUserId,
            fromInventory.BranchId,
            request.ToBranchId,
            fromInventory.WarehouseId,
            request.ToWarehouseId,
            cancellationToken);

        var transferInTransaction = await CreateTransactionAsync(
            toInventory.Id,
            "TRANSFER",
            request.Quantity,
            request.UnitCost,
            $"TRF-{toInventory.Id}:N8",
            request.Reason,
            request.TransferredByUserId,
            fromInventory.BranchId,
            request.ToBranchId,
            fromInventory.WarehouseId,
            request.ToWarehouseId,
            cancellationToken);

        logger.LogInformation("Inventory transferred successfully{Args}", new
        {       
            fromInventoryId = fromInventory.Id,
            toInventoryId = toInventory.Id,
            transferredQuantity = request.Quantity,
            transferredValue = request.Quantity * request.UnitCost
        });

        return new InventoryTransferResult
        {
            Success = true,
            FromInventoryId = fromInventory.Id,
            ToInventoryId = toInventory.Id,
            TransferredQuantity = request.Quantity,
            TransferredValue = request.Quantity * request.UnitCost,
            Message = "Transfer completed successfully",
            Transactions = [transferOutTransaction, transferInTransaction]
        };
    }

    public async Task<InventoryTransferResult> BulkTransferInventoryAsync(List<TransferInventoryRequest> requests, Guid userId, CancellationToken cancellationToken = default)
    {
        var results = new List<InventoryTransferResult>();
        var warnings = new List<string>();

        foreach (var request in requests)
        {
            var result = await TransferInventoryAsync(request, cancellationToken);
            results.Add(result);
            if (!result.Success)
            {
                warnings.Add(result.Message);
            }
        }

        return new InventoryTransferResult
        {
            Success = results.All(r => r.Success),
            FromInventoryId = Guid.Empty,
            ToInventoryId = Guid.Empty,
            TransferredQuantity = results.Where(r => r.Success).Sum(r => r.TransferredQuantity),
            TransferredValue = results.Where(r => r.Success).Sum(r => r.TransferredValue),
            Message = warnings.Any() ? string.Join("; ", warnings) : "Bulk transfer completed",
            Warnings = warnings,
            Transactions = results.SelectMany(r => r.Transactions).ToList()
        };
    }

    // Adjustment operations
    public async Task<Inventory> AdjustInventoryAsync(Guid inventoryId, AdjustInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(inventoryId, cancellationToken);
        if (inventory == null)
        {
            throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
        }

        // Validate request
        var validationResult = await ValidateAdjustmentRequest(inventoryId, request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validationResult.Errors));
        }

        var oldQuantity = inventory.Quantity;
        inventory.Quantity = request.Quantity;
        inventory.AvailableQuantity = inventory.Quantity - inventory.ReservedQuantity;
        inventory.UnitCost = request.UnitCost;
        inventory.LastUpdated = DateTime.UtcNow;

        await inventoryRepository.UpdateQuantityAsync(inventoryId, inventory.Quantity, inventory.UnitCost, request.Reason, request.AdjustedByUserId, cancellationToken);

        // Create adjustment transaction
        var quantityDiff = request.Quantity - oldQuantity;
        await CreateTransactionAsync(
            inventoryId,
            "ADJUSTMENT",
            Math.Abs(quantityDiff),
            request.UnitCost,
            $"ADJ-{inventoryId:N8}",
            request.Reason,
            request.AdjustedByUserId,
            null,
            null,
            null,
            null,
            cancellationToken);

        logger.LogInformation("Inventory adjusted successfully with ID: {InventoryId}, OldQuantity: {OldQuantity}, NewQuantity: {NewQuantity}", inventoryId, oldQuantity, request.Quantity);

        return inventory;
    }

    public async Task<IEnumerable<Inventory>> BulkAdjustInventoryAsync(BulkInventoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var results = new List<Inventory>();
        var warnings = new List<string>();

        foreach (var item in request.Items)
        {
            try
            {
                var result = await AdjustInventoryAsync(item.InventoryId, new AdjustInventoryRequest
                {
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    Reason = request.Reason,
                    AdjustedByUserId = request.UpdatedByUserId
                }, cancellationToken);
                results.Add(result);
            }
            catch (Exception ex)
            {
                warnings.Add($"Failed to adjust inventory {item.InventoryId}: {ex.Message}");
            }
        }

        if (warnings.Any())
        {
            logger.LogWarning("Some inventory adjustments failed", new { warnings });
        }

        return results;
    }

    // Search and filtering
    public async Task<(IEnumerable<Inventory> items, int totalCount)> SearchInventoryAsync(InventorySearchRequest request, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetWithPaginationAsync(
            request.Page,
            request.PageSize,
            request.BranchId,
            request.WarehouseId,
            request.ProductId,
            request.ProductVariationId,
            request.LowStock,
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetLowStockItemsAsync(threshold,null, null, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetOutOfStockItemsAsync(cancellationToken);
    }

    // Statistics and reporting
    public async Task<InventoryStatistics> GetInventoryStatisticsAsync(
        Guid? branchId = null, 
        Guid? warehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetStatisticsAsync(branchId, warehouseId, cancellationToken);
    }

    public async Task<IEnumerable<LocationInventoryValue>> GetInventoryValueByLocationAsync(CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetInventoryValueByLocationAsync(cancellationToken);
    }

    public async Task<InventoryReport> GenerateInventoryReportAsync(
        Guid? branchId = null, 
        Guid? warehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        var statistics = await GetInventoryStatisticsAsync(branchId, warehouseId, cancellationToken);
        var locationValues = await GetInventoryValueByLocationAsync(cancellationToken);
        var transactionSummaries = await transactionRepository.GetTransactionSummaryAsync(
            DateTime.UtcNow.AddDays(-30), 
            DateTime.UtcNow, 
            branchId, 
            warehouseId, 
            cancellationToken);

        var lowStockAlerts = await GetLowStockAlertsAsync(branchId, warehouseId,cancellationToken);
        var recentMovements = await transactionRepository.GetMovementsAsync(
            DateTime.UtcNow.AddDays(-7), 
            DateTime.UtcNow, 
            branchId, 
            warehouseId, 
            cancellationToken);

        return new InventoryReport
        {
            
            Statistics = statistics,
            LocationValues = locationValues.ToList(),
            TransactionSummaries = transactionSummaries.ToList(),
            LowStockAlerts = lowStockAlerts.ToList(),
            RecentMovements = recentMovements.ToList(),
            ReportDate = DateTime.UtcNow,
            GeneratedBy = "Inventory Management Service"
        };
    }

    // Transaction management
    public async Task<InventoryTransaction> CreateTransactionAsync(
        Guid inventoryId, 
        string transactionType, 
        int quantity, 
        decimal unitCost, 
        string referenceNumber, 
        string reason, 
        Guid createdByUserId, 
        Guid? fromBranchId = null, 
        Guid? toBranchId = null, 
        Guid? fromWarehouseId = null, 
        Guid? toWarehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        var transaction = new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            InventoryId = inventoryId,
            TransactionType = transactionType,
            Quantity = quantity,
            UnitCost = unitCost,
            ReferenceNumber = referenceNumber,
            Reason = reason,
            CreatedByUserId = createdByUserId,
            FromBranchId = fromBranchId,
            ToBranchId = toBranchId,
            FromWarehouseId = fromWarehouseId,
            ToWarehouseId = toWarehouseId,
            CreatedAt = DateTime.UtcNow
        };

        return await transactionRepository.AddAsync(transaction, cancellationToken);
    }

    public async Task<(IEnumerable<InventoryTransaction> items, int totalCount)> SearchTransactionsAsync(InventoryTransactionSearchRequest request, CancellationToken cancellationToken = default)
    {
        return await transactionRepository.GetWithPaginationAsync(
            request.Page,
            request.PageSize,
            request.InventoryId,
            request.ProductId,
            request.ProductVariationId,
            request.BranchId,
            request.WarehouseId,
            request.TransactionType,
            request.StartDate,
            request.EndDate,
            request.UserId,
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<InventoryTransaction>> GetRecentTransactionsAsync(
        int count = 10, 
        Guid? branchId = null, 
        Guid? warehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        return await transactionRepository.GetRecentTransactionsAsync(count, branchId, warehouseId, cancellationToken);
    }

    // Validation
    public async Task<ValidationResult> ValidateInventoryRequest(CreateInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Validate required fields
        if (request.Quantity < 0)
            errors.Add("Quantity must be greater than or equal to 0");

        if (request.UnitCost < 0)
            errors.Add("Unit cost must be greater than or equal to 0");

        if (string.IsNullOrWhiteSpace(request.Reason))
            errors.Add("Reason is required");

        // Validate product exists
        var product = await productCatalogService.GetProductAsync(request.ProductId, cancellationToken);
        if (product == null)
            errors.Add("Product not found");

        // Validate variation exists if specified
        if (request.ProductVariationId.HasValue)
        {
            var variationsResult = await productCatalogService.GetProductVariationsAsync(request.ProductId, new GetProductVariationsRequest(), cancellationToken);
            var variations = variationsResult.variations;
            if (variations.All(v => v.Id != request.ProductVariationId))
                errors.Add("Product variation not found");
        }

        // Validate location exists
         var branch = await productCatalogService.GetBranchAsync(request.BranchId, cancellationToken);
         if (branch == null) 
             errors.Add("Branch not found");
        
        // Validate warehouse exists if specified
        if (request.WarehouseId.HasValue)
        {
            var warehouse = await productCatalogService.GetWarehouseAsync(request.WarehouseId.Value, cancellationToken);
            if (warehouse == null)
                errors.Add("Warehouse not found");
        }

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }

     public async Task<ValidationResult> ValidateInventoryRequest(UpdateInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Validate required fields
        if (request.Quantity < 0)
            errors.Add("Quantity must be greater than or equal to 0");

        if (request.UnitCost < 0)
            errors.Add("Unit cost must be greater than or equal to 0");

        if (string.IsNullOrWhiteSpace(request.Reason))
            errors.Add("Reason is required");

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
    public async Task<ValidationResult> ValidateTransferRequest(TransferInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Validate source inventory exists
        var fromInventory = await inventoryRepository.GetByIdAsync(request.FromInventoryId, cancellationToken);
        if (fromInventory == null)
            errors.Add("Source inventory not found");

        // Validate destination location exists
        var destinationBranch = await productCatalogService.GetBranchAsync(request.ToBranchId, cancellationToken);
        if (destinationBranch == null)
            errors.Add("Destination branch not found");

        // Validate warehouse exists if specified
        if (request.ToWarehouseId.HasValue)
        {
            var destinationWarehouse = await productCatalogService.GetWarehouseAsync(request.ToWarehouseId.Value, cancellationToken);
            if (destinationWarehouse == null)
                errors.Add("Destination warehouse not found");
        }

        // Validate quantity
        if (request.Quantity <= 0)
            errors.Add("Transfer quantity must be greater than 0");

        // Validate sufficient quantity available
        if (fromInventory.AvailableQuantity < request.Quantity)
            errors.Add($"Insufficient quantity available. Available: {fromInventory.AvailableQuantity}, Requested: {request.Quantity}");

        // Validate unit cost
        if (request.UnitCost < 0)
            errors.Add("Unit cost must be greater than or equal to 0");

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }


    public async Task<ValidationResult> ValidateAdjustmentRequest(Guid inventoryId,  AdjustInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Validate inventory exists
        var inventory = await inventoryRepository.GetByIdAsync(inventoryId, cancellationToken);
        if (inventory == null)
            errors.Add("Inventory not found");

        // Validate quantity
        if (request.Quantity < -1000 || request.Quantity > 1000)
            errors.Add("Adjustment quantity must be between -1000 and 1000");

        // Validate unit cost
        if (request.UnitCost < 0)
            errors.Add("Unit cost must be greater than or equal to 0");

        // Validate reason
        if (string.IsNullOrWhiteSpace(request.Reason))
            errors.Add("Reason is required");

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }

    // Low stock alerts
    public async Task<IEnumerable<LowStockAlert>> GetLowStockAlertsAsync(Guid? branchId = null , Guid? warehouseId = null,CancellationToken cancellationToken = default)
    {
        var lowStockItems = await inventoryRepository.GetLowStockItemsAsync(10, branchId , warehouseId, cancellationToken);
        var alerts = new List<LowStockAlert>();

        foreach (var item in lowStockItems)
        {
            alerts.Add(new LowStockAlert
            {
                InventoryId = item.Id,
                ProductId = item.ProductId,
                ProductVariationId = item.ProductVariationId,
                ProductName = item.Product?.Name ?? "Unknown",
                ProductSKU = item.Product?.SKU ?? "",
                ProductVariationSize = item.ProductVariation?.Size,
                ProductVariationColor = item.ProductVariation?.Color,
                BranchId = item.BranchId,
                WarehouseId = item.WarehouseId,
                LocationName = item.Branch?.Name ?? (item.Warehouse?.Name ?? "Unknown"),
                CurrentQuantity = item.AvailableQuantity,
                LowStockThreshold = 10,
                ReorderPoint = 5,
                UnitCost = item.UnitCost,
                TotalValue = item.AvailableQuantity * item.UnitCost,
                AlertDate = DateTime.UtcNow,
                IsResolved = false,
                ResolvedDate = null
            });
        }

        return alerts;
    }

    public async Task SendLowStockAlertsAsync(CancellationToken cancellationToken = default)
    {
        var alerts = await GetLowStockAlertsAsync(null, null,cancellationToken);
        
        foreach (var alert in alerts.Where(a => !a.IsResolved))
        {
            // Send notification (email, SMS, push notification, etc.)
            logger.LogWarning("Low stock alert", new { alert });
        }
    }

    public async Task<object> GetInventorySummaryAsync(Guid? branchId = null, Guid? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var stats = await GetInventoryStatisticsAsync(branchId, warehouseId, cancellationToken);
        
        return new
        {
            TotalItems = stats.TotalItems,
            TotalValue = stats.TotalValue,
            LowStockCount = stats.LowStockItems,
            OutOfStockCount = stats.OutOfStockItems,
            BranchCounts = new Dictionary<Guid, int>(),
            WarehouseCounts = new Dictionary<Guid, int>()
        };
    }

    public async Task<IEnumerable<InventoryMovement>> GetRecentMovementsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        return await transactionRepository.GetMovementsAsync(startDate, endDate, branchId, warehouseId, cancellationToken);
    }
}
