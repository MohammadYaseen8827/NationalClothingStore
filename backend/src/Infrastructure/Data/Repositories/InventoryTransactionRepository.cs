using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for InventoryTransaction entity
/// </summary>
public class InventoryTransactionRepository(NationalClothingStoreDbContext context)
    : Repository<InventoryTransaction>(context), IInventoryTransactionRepository
{
    /// <summary>
    /// Get inventory transaction by ID with related entities
    /// </summary>
    public new async Task<InventoryTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Include(t => t.FromBranch)
            .Include(t => t.ToBranch)
            .Include(t => t.FromWarehouse)
            .Include(t => t.ToWarehouse)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get transactions for a specific inventory item
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.InventoryId == inventoryId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions for a specific product across all locations
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.Inventory.ProductId == productId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions for a specific product variation across all locations
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByProductVariationIdAsync(Guid productVariationId, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.Inventory.ProductVariationId == productVariationId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions for a specific branch
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.Inventory.BranchId == branchId || t.ToBranchId == branchId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions for a specific warehouse
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.Inventory.WarehouseId == warehouseId || t.ToWarehouseId == warehouseId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions by type
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByTransactionTypeAsync(string transactionType, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.TransactionType == transactionType)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions by date range
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions by user
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.CreatedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions with pagination and filtering
    /// </summary>
    public async Task<(IEnumerable<InventoryTransaction> items, int totalCount)> GetWithPaginationAsync(
        int page,
        int pageSize,
        Guid? inventoryId = null,
        Guid? productId = null,
        Guid? productVariationId = null,
        Guid? branchId = null,
        Guid? warehouseId = null,
        string? transactionType = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .AsQueryable();

        if (inventoryId.HasValue)
            query = query.Where(t => t.InventoryId == inventoryId.Value);

        if (productId.HasValue)
            query = query.Where(t => t.Inventory.ProductId == productId.Value);

        if (productVariationId.HasValue)
            query = query.Where(t => t.Inventory.ProductVariationId == productVariationId.Value);

        if (branchId.HasValue)
            query = query.Where(t => t.Inventory.BranchId == branchId.Value || t.ToBranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(t => t.Inventory.WarehouseId == warehouseId.Value || t.ToWarehouseId == warehouseId.Value);

        if (!string.IsNullOrEmpty(transactionType))
            query = query.Where(t => t.TransactionType == transactionType);

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        if (userId.HasValue)
            query = query.Where(t => t.CreatedByUserId == userId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Get transaction summary by type
    /// </summary>
    public async Task<IEnumerable<TransactionSummary>> GetTransactionSummaryAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        if (branchId.HasValue)
            query = query.Where(t => t.Inventory.BranchId == branchId.Value || t.ToBranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(t => t.Inventory.WarehouseId == warehouseId.Value || t.ToWarehouseId == warehouseId.Value);

        return await query
            .GroupBy(t => t.TransactionType)
            .Select(g => new TransactionSummary
            {
                TransactionType = g.Key,
                TransactionCount = g.Count(),
                TotalQuantity = g.Sum(t => t.Quantity),
                TotalValue = g.Sum(t => t.Quantity * t.UnitCost),
                AverageQuantity = (decimal)g.Average(t => t.Quantity),
                AverageValue = g.Average(t => t.Quantity * t.UnitCost)
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get recent transactions
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetRecentTransactionsAsync(
        int count = 10,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(t => t.Inventory.BranchId == branchId.Value || t.ToBranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(t => t.Inventory.WarehouseId == warehouseId.Value || t.ToWarehouseId == warehouseId.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get movements (transactions) by date range and location
    /// </summary>
    public async Task<IEnumerable<InventoryMovement>> GetMovementsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Include(t => t.FromBranch)
            .Include(t => t.ToBranch)
            .Include(t => t.FromWarehouse)
            .Include(t => t.ToWarehouse)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);

        if (branchId.HasValue)
            query = query.Where(t => t.Inventory.BranchId == branchId.Value || t.ToBranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(t => t.Inventory.WarehouseId == warehouseId.Value || t.ToWarehouseId == warehouseId.Value);

        var transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        // Map to InventoryMovement objects
        return transactions.Select(t => new InventoryMovement
        {
            TransactionId = t.Id,
            ProductId = t.Inventory.Product.Id,
            ProductVariationId = t.Inventory.ProductVariation?.Id,
            ProductName = t.Inventory.Product.Name,
            ProductSku = t.Inventory.Product.SKU,
            ProductVariationSize = t.Inventory.ProductVariation?.Size,
            ProductVariationColor = t.Inventory.ProductVariation?.Color,
            TransactionType = t.TransactionType,
            Quantity = t.Quantity,
            UnitCost = t.UnitCost,
            ReferenceNumber = t.ReferenceNumber,
            Reason = t.Reason,
            FromBranchId = t.FromBranchId,
            ToBranchId = t.ToBranchId,
            FromWarehouseId = t.FromWarehouseId,
            ToWarehouseId = t.ToWarehouseId,
            CreatedAt = t.CreatedAt,
            CreatedByUserId = t.CreatedByUserId
        });
    }

    /// <summary>
    /// Get transactions by reference number
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.ReferenceNumber == referenceNumber)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transfer transactions
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetTransferTransactionsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.TransactionType == "TRANSFER")
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get adjustment transactions
    /// </summary>
    public async Task<IEnumerable<InventoryTransaction>> GetAdjustmentTransactionsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Include(t => t.CreatedByUser)
            .Where(t => t.TransactionType == "ADJUSTMENT")
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transaction statistics
    /// </summary>
    public async Task<TransactionStatistics> GetStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        if (branchId.HasValue)
            query = query.Where(t => t.Inventory.BranchId == branchId.Value || t.ToBranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(t => t.Inventory.WarehouseId == warehouseId.Value || t.ToWarehouseId == warehouseId.Value);

        var transactions = await query.ToListAsync(cancellationToken);

        if (!transactions.Any())
        {
            return new TransactionStatistics
            {
                FirstTransactionDate = DateTime.UtcNow,
                LastTransactionDate = DateTime.UtcNow
            };
        }

        var inTransactions = transactions.Where(t => t.TransactionType == "IN").ToList();
        var outTransactions = transactions.Where(t => t.TransactionType == "OUT").ToList();
        var transferTransactions = transactions.Where(t => t.TransactionType == "TRANSFER").ToList();
        var adjustmentTransactions = transactions.Where(t => t.TransactionType == "ADJUSTMENT").ToList();

        return new TransactionStatistics
        {
            TotalTransactions = transactions.Count,
            InTransactions = inTransactions.Count,
            OutTransactions = outTransactions.Count,
            TransferTransactions = transferTransactions.Count,
            AdjustmentTransactions = adjustmentTransactions.Count,
            TotalQuantityIn = inTransactions.Sum(t => t.Quantity),
            TotalQuantityOut = outTransactions.Sum(t => t.Quantity),
            TotalValueIn = inTransactions.Sum(t => t.Quantity * t.UnitCost),
            TotalValueOut = outTransactions.Sum(t => t.Quantity * t.UnitCost),
            NetQuantityChange = inTransactions.Sum(t => t.Quantity) - outTransactions.Sum(t => t.Quantity),
            NetValueChange = inTransactions.Sum(t => t.Quantity * t.UnitCost) - outTransactions.Sum(t => t.Quantity * t.UnitCost),
            FirstTransactionDate = transactions.Min(t => t.CreatedAt),
            LastTransactionDate = transactions.Max(t => t.CreatedAt),
            TransactionsByType = transactions
                .GroupBy(t => t.TransactionType)
                .ToDictionary(g => g.Key, g => g.Count()),
            TransactionsByDay = transactions
                .GroupBy(t => t.CreatedAt.Date)
                .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Count())
        };
    }
}
