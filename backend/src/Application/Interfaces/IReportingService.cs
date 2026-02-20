using NationalClothingStore.Application.Services;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Service interface for generating business reports
/// </summary>
public interface IReportingService
{
    /// <summary>
    /// Generate sales report for a specific date range
    /// </summary>
    Task<SalesReport> GenerateSalesReportAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate inventory report for a specific location
    /// </summary>
    Task<InventoryReport> GenerateInventoryReportAsync(
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate customer report for a specific date range
    /// </summary>
    Task<CustomerReport> GenerateCustomerReportAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate procurement report for a specific date range
    /// </summary>
    Task<ProcurementReport> GenerateProcurementReportAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate financial report for a specific date range
    /// </summary>
    Task<FinancialReport> GenerateFinancialReportAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
