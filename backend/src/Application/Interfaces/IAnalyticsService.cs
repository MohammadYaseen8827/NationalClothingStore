using NationalClothingStore.Application.Services;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Service interface for advanced analytics and business intelligence
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Get comprehensive sales analytics with growth metrics and trends
    /// </summary>
    Task<SalesAnalytics> GetSalesAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        AnalyticsPeriod period = AnalyticsPeriod.Daily,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get inventory analytics with turnover, health, and demand forecasting
    /// </summary>
    Task<InventoryAnalytics> GetInventoryAnalyticsAsync(
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customer analytics with acquisition, retention, and lifetime value metrics
    /// </summary>
    Task<CustomerAnalytics> GetCustomerAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get financial analytics with profitability, cash flow, and ratio analysis
    /// </summary>
    Task<FinancialAnalytics> GetFinancialAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        AnalyticsPeriod period = AnalyticsPeriod.Monthly,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get predictive analytics for forecasting and trend analysis
    /// </summary>
    Task<PredictiveAnalytics> GetPredictiveAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        PredictionType predictionType,
        CancellationToken cancellationToken = default);
}
