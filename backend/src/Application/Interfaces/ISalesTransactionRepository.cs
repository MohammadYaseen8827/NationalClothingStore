using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for SalesTransaction entity operations
/// </summary>
public interface ISalesTransactionRepository
{
    /// <summary>
    /// Get sales transaction by ID with navigation properties
    /// </summary>
    Task<SalesTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales transaction by transaction number
    /// </summary>
    Task<SalesTransaction?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales transactions with pagination and filtering
    /// </summary>
    Task<(IEnumerable<SalesTransaction> transactions, int totalCount)> GetPagedAsync(
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
    /// Get all sales transactions
    /// </summary>
    Task<IEnumerable<SalesTransaction>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales transactions by customer
    /// </summary>
    Task<IEnumerable<SalesTransaction>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales transactions by branch
    /// </summary>
    Task<IEnumerable<SalesTransaction>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new sales transaction
    /// </summary>
    Task<SalesTransaction> CreateAsync(SalesTransaction transaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing sales transaction
    /// </summary>
    Task<SalesTransaction> UpdateAsync(SalesTransaction transaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a sales transaction
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if sales transaction exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if transaction number already exists
    /// </summary>
    Task<bool> TransactionNumberExistsAsync(string transactionNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales transaction count
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get sales statistics for a period
    /// </summary>
    Task<SalesStatistics> GetSalesStatisticsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
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
    /// Get daily sales summary
    /// </summary>
    Task<IEnumerable<DailySalesSummary>> GetDailySalesSummaryAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        CancellationToken cancellationToken = default);
}

// /// <summary>
// /// Sales statistics DTO
// /// </summary>
// public class SalesStatistics
// {
//     public decimal TotalSales { get; set; }
//     public int TotalTransactions { get; set; }
//     public decimal AverageTransactionValue { get; set; }
//     public int TotalItemsSold { get; set; }
//     public decimal TotalDiscounts { get; set; }
//     public decimal TotalTax { get; set; }
//     public int TotalCustomers { get; set; }
// }
//
// /// <summary>
// /// Product sales summary DTO
// /// </summary>
// public class ProductSalesSummary
// {
//     public Guid ProductId { get; set; }
//     public string ProductName { get; set; } = string.Empty;
//     public int QuantitySold { get; set; }
//     public decimal TotalRevenue { get; set; }
//     public decimal AveragePrice { get; set; }
// }

/// <summary>
/// Daily sales summary DTO
/// </summary>
public class DailySalesSummary
{
    public DateTime Date { get; set; }
    public decimal TotalSales { get; set; }
    public int TransactionCount { get; set; }
    public int ItemCount { get; set; }
    public decimal AverageTransactionValue { get; set; }
}