using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Service interface for processing sales transactions
/// </summary>
public interface ISalesProcessingService
{
    /// <summary>
    /// Process a new sale transaction
    /// </summary>
    Task<SalesTransaction> ProcessSaleAsync(ProcessSaleRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Process a return transaction
    /// </summary>
    Task<SalesTransaction> ProcessReturnAsync(ProcessReturnRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Process an exchange transaction (return + new sale)
    /// </summary>
    Task<SalesTransaction> ProcessExchangeAsync(ProcessExchangeRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales transaction by ID
    /// </summary>
    Task<SalesTransaction?> GetTransactionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales transaction by transaction number
    /// </summary>
    Task<SalesTransaction?> GetTransactionByNumberAsync(string transactionNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated sales transactions
    /// </summary>
    Task<(IEnumerable<SalesTransaction> transactions, int totalCount)> GetTransactionsAsync(
        int pageNumber,
        int pageSize,
        Guid? branchId = null,
        Guid? customerId = null,
        string? transactionType = null,
        string? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales statistics for a period
    /// </summary>
    Task<SalesStatistics> GetSalesStatisticsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all sales transactions for a period (for reporting)
    /// </summary>
    Task<IEnumerable<SalesTransaction>> GetSalesTransactionsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get top selling products
    /// </summary>
    Task<IEnumerable<ProductSalesSummary>> GetTopSellingProductsAsync(
        DateTime startDate,
        DateTime endDate,
        int limit = 10,
        Guid? branchId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancel a pending transaction
    /// </summary>
    Task<bool> CancelTransactionAsync(Guid transactionId, string reason, CancellationToken cancellationToken = default);
}

// Request/Response DTOs
public class ProcessSaleRequest
{
    public Guid BranchId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CustomerId { get; set; }
    public List<SaleItemRequest> Items { get; set; } = new();
    public List<SalePaymentRequest> Payments { get; set; } = new();
    public string? Notes { get; set; }
}

public class SaleItemRequest
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }
    public Guid InventoryId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxRate { get; set; } = 8.5m;
    public string? Notes { get; set; }
}

public class SalePaymentRequest
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? ReferenceNumber { get; set; }
    public string? CardLastFour { get; set; }
    public string? CardType { get; set; }
    public string? GiftCardNumber { get; set; }
    public string? AuthorizationCode { get; set; }
}

public class ProcessReturnRequest
{
    public string OriginalTransactionNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public List<ReturnItemRequest> Items { get; set; } = new();
    public SalePaymentRequest? RefundPayment { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ReturnItemRequest
{
    public Guid OriginalItemId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ProcessExchangeRequest
{
    public string OriginalTransactionNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public List<ReturnItemRequest> ReturnItems { get; set; } = new();
    public List<SaleItemRequest> NewItems { get; set; } = new();
    public List<SalePaymentRequest> Payments { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
}

public class SalesStatistics
{
    public decimal TotalSales { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public int TotalItemsSold { get; set; }
    public decimal TotalDiscounts { get; set; }
    public decimal TotalTax { get; set; }
    public int TotalCustomers { get; set; }
}

public class ProductSalesSummary
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePrice { get; set; }
}