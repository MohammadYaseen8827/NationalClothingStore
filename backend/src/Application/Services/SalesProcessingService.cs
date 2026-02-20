using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;
using NationalClothingStore.Application.Common;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service for processing sales transactions including sales, returns, and exchanges
/// </summary>
public class SalesProcessingService(
    ISalesTransactionRepository salesTransactionRepository,
    ICustomerRepository customerRepository,
    IInventoryRepository inventoryRepository,
    IInventoryTransactionRepository inventoryTransactionRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<SalesProcessingService> logger)
    : ISalesProcessingService
{
    public async Task<SalesTransaction> ProcessSaleAsync(ProcessSaleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            // Validate customer if provided
            Customer? customer = null;
            if (request.CustomerId.HasValue)
            {
                customer = await customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken);
                if (customer == null)
                {
                    throw new ValidationException($"Customer with ID '{request.CustomerId.Value}' not found.");
                }
            }

            // Create sales transaction
            var transaction = new SalesTransaction
            {
                TransactionNumber = GenerateTransactionNumber(),
                BranchId = request.BranchId,
                CustomerId = request.CustomerId,
                UserId = request.UserId,
                TransactionType = "SALE",
                Status = "PENDING",
                Notes = request.Notes
            };

            decimal subtotal = 0;
            decimal taxAmount = 0;
            var transactionItems = new List<SalesTransactionItem>();

            // Process each item
            foreach (var itemRequest in request.Items)
            {
                var transactionItem = await ProcessSaleItemAsync(itemRequest, transaction.Id, cancellationToken);
                transactionItems.Add(transactionItem);
                subtotal += transactionItem.PriceAfterDiscount;
                taxAmount += transactionItem.TaxAmount;
            }

            // Calculate totals
            transaction.Subtotal = subtotal;
            transaction.TaxAmount = taxAmount;
            transaction.TotalAmount = subtotal + taxAmount;
            transaction.AmountPaid = request.Payments.Sum(p => p.Amount);
            transaction.ChangeGiven = Math.Max(0, transaction.AmountPaid - transaction.TotalAmount);
            
            // Process loyalty points
            if (customer?.Loyalty is { IsActive: true })
            {
                var pointsEarned = CalculateLoyaltyPoints(transaction.TotalAmount, customer.Loyalty.TierDiscountPercentage);
                transaction.LoyaltyPointsEarned = pointsEarned;
            }

            // Save transaction
            var savedTransaction = await salesTransactionRepository.CreateAsync(transaction, cancellationToken);

            // Save transaction items
            foreach (var item in transactionItems)
            {
                item.SalesTransactionId = savedTransaction.Id;
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Process payments
            foreach (var paymentRequest in request.Payments)
            {
                var payment = new SalesTransactionPayment
                {
                    SalesTransactionId = savedTransaction.Id,
                    PaymentMethod = paymentRequest.PaymentMethod,
                    Amount = paymentRequest.Amount,
                    Currency = paymentRequest.Currency,
                    ReferenceNumber = paymentRequest.ReferenceNumber,
                    CardLastFour = paymentRequest.CardLastFour,
                    CardType = paymentRequest.CardType,
                    GiftCardNumber = paymentRequest.GiftCardNumber,
                    AuthorizationCode = paymentRequest.AuthorizationCode,
                    IsApproved = true
                };
                
                unitOfWork.Context.Set<SalesTransactionPayment>().Add(payment);
            }

            // Update inventory
            await UpdateInventoryForSaleAsync(transactionItems, request.UserId, cancellationToken);

            // Update loyalty points if applicable
            if (customer?.Loyalty != null && transaction.LoyaltyPointsEarned > 0)
            {
                await UpdateLoyaltyPointsAsync(customer.Loyalty.Id, transaction.LoyaltyPointsEarned, 
                    "EARNED", $"Purchase transaction {transaction.TransactionNumber}", 
                    savedTransaction.Id, cancellationToken);
            }

            transaction.Status = "COMPLETED";
            transaction.CompletedAt = DateTime.UtcNow;
            await salesTransactionRepository.UpdateAsync(transaction, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Sale processed successfully. Transaction: {TransactionNumber}", transaction.TransactionNumber);
            return savedTransaction;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Error processing sale");
            throw;
        }
    }

    public async Task<SalesTransaction> ProcessReturnAsync(ProcessReturnRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get original transaction
            var originalTransaction = await salesTransactionRepository.GetByTransactionNumberAsync(
                request.OriginalTransactionNumber, cancellationToken);
            
            if (originalTransaction == null)
            {
                throw new ValidationException($"Original transaction '{request.OriginalTransactionNumber}' not found.");
            }

            if (originalTransaction.Status != "COMPLETED")
            {
                throw new ValidationException($"Cannot return transaction with status '{originalTransaction.Status}'.");
            }

            // Create return transaction
            var returnTransaction = new SalesTransaction
            {
                TransactionNumber = GenerateTransactionNumber(),
                BranchId = originalTransaction.BranchId,
                CustomerId = originalTransaction.CustomerId,
                UserId = request.UserId,
                TransactionType = "RETURN",
                Status = "PENDING",
                OriginalTransactionId = originalTransaction.Id,
                Notes = request.Reason
            };

            decimal refundAmount = 0;
            var returnItems = new List<SalesTransactionItem>();

            // Process return items
            foreach (var itemRequest in request.Items)
            {
                var originalItem = originalTransaction.Items.FirstOrDefault(i => i.Id == itemRequest.OriginalItemId);
                if (originalItem == null)
                {
                    throw new ValidationException($"Original item with ID '{itemRequest.OriginalItemId}' not found.");
                }

                if (itemRequest.Quantity > originalItem.Quantity)
                {
                    throw new ValidationException($"Cannot return more items ({itemRequest.Quantity}) than originally purchased ({originalItem.Quantity}).");
                }

                var returnItem = new SalesTransactionItem
                {
                    ProductId = originalItem.ProductId,
                    ProductVariationId = originalItem.ProductVariationId,
                    InventoryId = originalItem.InventoryId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = originalItem.UnitPrice,
                    DiscountAmount = (originalItem.DiscountAmount / originalItem.Quantity) * itemRequest.Quantity,
                    TaxAmount = (originalItem.TaxAmount / originalItem.Quantity) * itemRequest.Quantity,
                    Notes = itemRequest.Reason
                };

                returnItem.TotalPrice = -(returnItem.Quantity * returnItem.UnitPrice - returnItem.DiscountAmount + returnItem.TaxAmount);
                refundAmount += Math.Abs(returnItem.TotalPrice);
                returnItems.Add(returnItem);
            }

            returnTransaction.Subtotal = -refundAmount;
            returnTransaction.TaxAmount = -returnItems.Sum(i => i.TaxAmount);
            returnTransaction.TotalAmount = -refundAmount;
            returnTransaction.AmountPaid = -refundAmount;

            // Save return transaction
            var savedReturnTransaction = await salesTransactionRepository.CreateAsync(returnTransaction, cancellationToken);

            // Save return items
            foreach (var item in returnItems)
            {
                item.SalesTransactionId = savedReturnTransaction.Id;
                unitOfWork.Context.Set<SalesTransactionItem>().Add(item);
            }

            // Process refund payment
            if (request.RefundPayment != null)
            {
                var refundPayment = new SalesTransactionPayment
                {
                    SalesTransactionId = savedReturnTransaction.Id,
                    PaymentMethod = request.RefundPayment.PaymentMethod,
                    Amount = -refundAmount,
                    Currency = request.RefundPayment.Currency,
                    ReferenceNumber = request.RefundPayment.ReferenceNumber,
                    CardLastFour = request.RefundPayment.CardLastFour,
                    CardType = request.RefundPayment.CardType,
                    IsApproved = true
                };
                unitOfWork.Context.Set<SalesTransactionPayment>().Add(refundPayment);
            }

            // Restore inventory
            await RestoreInventoryForReturnAsync(returnItems, request.UserId, cancellationToken);

            // Deduct loyalty points if they were earned on original transaction
            if (originalTransaction is { LoyaltyPointsEarned: > 0, CustomerId: not null })
            {
                var customer = await customerRepository.GetByIdAsync(originalTransaction.CustomerId.Value, cancellationToken);
                if (customer?.Loyalty != null)
                {
                    await UpdateLoyaltyPointsAsync(customer.Loyalty.Id, -originalTransaction.LoyaltyPointsEarned,
                        "REDEEMED", $"Return of transaction {originalTransaction.TransactionNumber}",
                        savedReturnTransaction.Id, cancellationToken);
                }
            }

            returnTransaction.Status = "COMPLETED";
            returnTransaction.CompletedAt = DateTime.UtcNow;
            await salesTransactionRepository.UpdateAsync(returnTransaction, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Return processed successfully. Return Transaction: {TransactionNumber}", returnTransaction.TransactionNumber);
            return savedReturnTransaction;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Error processing return");
            throw;
        }
    }

    private async Task<SalesTransactionItem> ProcessSaleItemAsync(SaleItemRequest itemRequest, Guid transactionId, CancellationToken cancellationToken)
    {
        // Get product and inventory
        var product = await productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
        if (product == null)
        {
            throw new ValidationException($"Product with ID '{itemRequest.ProductId}' not found.");
        }

        var inventory = await inventoryRepository.GetByIdAsync(itemRequest.InventoryId, cancellationToken);
        if (inventory == null)
        {
            throw new ValidationException($"Inventory with ID '{itemRequest.InventoryId}' not found.");
        }

        if (inventory.ProductId != itemRequest.ProductId)
        {
            throw new ValidationException("Inventory does not match the product.");
        }

        // Check stock availability
        if (inventory.AvailableQuantity < itemRequest.Quantity)
        {
            throw new ValidationException($"Insufficient stock. Available: {inventory.AvailableQuantity}, Requested: {itemRequest.Quantity}");
        }

        // Get price (could come from inventory, product, or be specified)
        var unitPrice = itemRequest.UnitPrice > 0 ? itemRequest.UnitPrice : inventory.UnitCost * 1.5m; // Default markup

        var transactionItem = new SalesTransactionItem
        {
            SalesTransactionId = transactionId,
            ProductId = itemRequest.ProductId,
            ProductVariationId = itemRequest.ProductVariationId,
            InventoryId = itemRequest.InventoryId,
            Quantity = itemRequest.Quantity,
            UnitPrice = unitPrice,
            DiscountAmount = itemRequest.DiscountAmount,
            TaxAmount = CalculateTax(unitPrice * itemRequest.Quantity - itemRequest.DiscountAmount, itemRequest.TaxRate),
            Notes = itemRequest.Notes
        };

        transactionItem.TotalPrice = transactionItem.Quantity * transactionItem.UnitPrice 
            - transactionItem.DiscountAmount + transactionItem.TaxAmount;

        return transactionItem;
    }

    private async Task UpdateInventoryForSaleAsync(List<SalesTransactionItem> items, Guid userId, CancellationToken cancellationToken)
    {
        foreach (var item in items)
        {
            var inventory = await inventoryRepository.GetByIdAsync(item.InventoryId, cancellationToken);
            if (inventory != null)
            {
                inventory.AvailableQuantity -= item.Quantity;
                inventory.ReservedQuantity = Math.Max(0, inventory.ReservedQuantity - item.Quantity);
                await inventoryRepository.UpdateAsync(inventory, cancellationToken);

                // Create inventory transaction
                var inventoryTransaction = new InventoryTransaction
                {
                    InventoryId = inventory.Id,
                    TransactionType = "SALE",
                    Quantity = -item.Quantity,
                    UnitCost = inventory.UnitCost,
                    ReferenceNumber = $"SALE-{Guid.NewGuid():N}",
                    Reason = $"Sale of {item.Quantity} units",
                    CreatedByUserId = userId
                };
                await inventoryTransactionRepository.AddAsync(inventoryTransaction, cancellationToken);
            }
        }
    }

    private async Task RestoreInventoryForReturnAsync(List<SalesTransactionItem> items, Guid userId, CancellationToken cancellationToken)
    {
        foreach (var item in items)
        {
            var inventory = await inventoryRepository.GetByIdAsync(item.InventoryId, cancellationToken);
            if (inventory != null)
            {
                inventory.AvailableQuantity += item.Quantity;
                await inventoryRepository.UpdateAsync(inventory, cancellationToken);

                // Create inventory transaction
                var inventoryTransaction = new InventoryTransaction
                {
                    InventoryId = inventory.Id,
                    TransactionType = "RETURN",
                    Quantity = item.Quantity,
                    UnitCost = inventory.UnitCost,
                    ReferenceNumber = $"RETURN-{Guid.NewGuid():N}",
                    Reason = $"Return of {item.Quantity} units",
                    CreatedByUserId = userId
                };
                await inventoryTransactionRepository.AddAsync(inventoryTransaction, cancellationToken);
            }
        }
    }

    private async Task UpdateLoyaltyPointsAsync(Guid customerLoyaltyId, int points, string transactionType, 
        string reason, Guid salesTransactionId, CancellationToken cancellationToken)
    {
        var loyaltyTransaction = new LoyaltyTransaction
        {
            CustomerLoyaltyId = customerLoyaltyId,
            Points = points,
            TransactionType = transactionType,
            Reason = reason,
            SalesTransactionId = salesTransactionId,
            TransactionDate = DateTime.UtcNow
        };

        var loyaltyDbSet = unitOfWork.Context.Set<LoyaltyTransaction>();
        loyaltyDbSet.Add(loyaltyTransaction);

        // Update customer loyalty record
        var customerLoyalty = await unitOfWork.Context.Set<CustomerLoyalty>()
            .FindAsync(new object[] { customerLoyaltyId }, cancellationToken);
        
        if (customerLoyalty != null)
        {
            customerLoyalty.PointsBalance += points;
            customerLoyalty.TotalPointsEarned += Math.Max(0, points);
            customerLoyalty.PointsRedeemed += Math.Max(0, -points);
            customerLoyalty.LastActivityDate = DateTime.UtcNow;
            customerLoyalty.UpdatedAt = DateTime.UtcNow;
        }
    }

    private string GenerateTransactionNumber()
    {
        return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }

    private int CalculateLoyaltyPoints(decimal amount, decimal discountPercentage)
    {
        // Example: 1 point per $1 spent, adjusted for tier discount
        var basePoints = (int)(amount * (1 - discountPercentage / 100));
        return Math.Max(0, basePoints);
    }

    private decimal CalculateTax(decimal amount, decimal taxRate)
    {
        return amount * (taxRate / 100);
    }

    public async Task<SalesTransaction?> GetTransactionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await salesTransactionRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<SalesTransaction?> GetTransactionByNumberAsync(string transactionNumber, CancellationToken cancellationToken = default)
    {
        return await salesTransactionRepository.GetByTransactionNumberAsync(transactionNumber, cancellationToken);
    }

    public async Task<(IEnumerable<SalesTransaction> transactions, int totalCount)> GetTransactionsAsync(
        int pageNumber,
        int pageSize,
        Guid? branchId = null,
        Guid? customerId = null,
        string? transactionType = null,
        string? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        return await salesTransactionRepository.GetPagedAsync(
            pageNumber, pageSize, branchId, customerId, transactionType, status, startDate, endDate, cancellationToken);
    }

    public async Task<SalesStatistics> GetSalesStatisticsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var dbStats = await salesTransactionRepository.GetSalesStatisticsAsync(startDate, endDate, branchId, cancellationToken);
        return new SalesStatistics
        {
            TotalSales = dbStats.TotalSales,
            TotalTransactions = dbStats.TotalTransactions,
            AverageTransactionValue = dbStats.AverageTransactionValue,
            TotalItemsSold = dbStats.TotalItemsSold,
            TotalDiscounts = dbStats.TotalDiscounts,
            TotalTax = dbStats.TotalTax,
            TotalCustomers = dbStats.TotalCustomers
        };
    }

    public async Task<IEnumerable<ProductSalesSummary>> GetTopSellingProductsAsync(
        DateTime startDate,
        DateTime endDate,
        int limit = 10,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSummaries = await salesTransactionRepository.GetTopSellingProductsAsync(startDate, endDate, limit, branchId, cancellationToken);
        return dbSummaries.Select(s => new ProductSalesSummary
        {
            ProductId = s.ProductId,
            ProductName = s.ProductName,
            QuantitySold = s.QuantitySold,
            TotalRevenue = s.TotalRevenue,
            AveragePrice = s.AveragePrice
        });
    }

    public async Task<IEnumerable<SalesTransaction>> GetSalesTransactionsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        // Get all transactions without pagination for reporting
        var (transactions, _) = await GetTransactionsAsync(
            pageNumber: 1,
            pageSize: int.MaxValue,
            branchId: branchId,
            startDate: startDate,
            endDate: endDate,
            cancellationToken: cancellationToken);

        return transactions;
    }

    public async Task<bool> CancelTransactionAsync(Guid transactionId, string reason, CancellationToken cancellationToken = default)
    {
        var transaction = await salesTransactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (transaction == null)
        {
            return false;
        }

        if (transaction.Status != "PENDING")
        {
            throw new ValidationException($"Cannot cancel transaction with status '{transaction.Status}'. Only pending transactions can be cancelled.");
        }

        transaction.Status = "CANCELLED";
        transaction.Notes = reason;
        transaction.UpdatedAt = DateTime.UtcNow;

        await salesTransactionRepository.UpdateAsync(transaction, cancellationToken);
        return true;
    }

    public async Task<SalesTransaction> ProcessExchangeAsync(ProcessExchangeRequest request, CancellationToken cancellationToken = default)
    {
        // First process the return
        var returnRequest = new ProcessReturnRequest
        {
            OriginalTransactionNumber = request.OriginalTransactionNumber,
            UserId = request.UserId,
            Items = request.ReturnItems,
            Reason = request.Reason
        };

        var returnTransaction = await ProcessReturnAsync(returnRequest, cancellationToken);

        // Then process the new sale
        var saleRequest = new ProcessSaleRequest
        {
            BranchId = returnTransaction.BranchId,
            UserId = request.UserId,
            CustomerId = returnTransaction.CustomerId,
            Items = request.NewItems,
            Payments = request.Payments,
            Notes = $"Exchange for transaction {request.OriginalTransactionNumber}" + (string.IsNullOrEmpty(request.Reason) ? "" : $" - {request.Reason}")
        };

        var newSaleTransaction = await ProcessSaleAsync(saleRequest, cancellationToken);

        // Link the transactions
        newSaleTransaction.OriginalTransactionId = returnTransaction.Id;
        await salesTransactionRepository.UpdateAsync(newSaleTransaction, cancellationToken);

        return newSaleTransaction;
    }
}