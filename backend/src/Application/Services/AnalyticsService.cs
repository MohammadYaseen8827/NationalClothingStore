using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service for advanced analytics and business intelligence
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly IReportingService _reportingService;
    private readonly IInventoryManagementService _inventoryManagementService;
    private readonly ISalesProcessingService _salesProcessingService;
    private readonly ICustomerManagementService _customerManagementService;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        IReportingService reportingService,
        IInventoryManagementService inventoryManagementService,
        ISalesProcessingService salesProcessingService,
        ICustomerManagementService customerManagementService,
        ILogger<AnalyticsService> logger)
    {
        _reportingService = reportingService;
        _inventoryManagementService = inventoryManagementService;
        _salesProcessingService = salesProcessingService;
        _customerManagementService = customerManagementService;
        _logger = logger;
    }

    public async Task<SalesAnalytics> GetSalesAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        AnalyticsPeriod period = AnalyticsPeriod.Daily,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating sales analytics from {StartDate} to {EndDate}", startDate, endDate);

            var salesReport = await _reportingService.GenerateSalesReportAsync(startDate, endDate, branchId, warehouseId, cancellationToken);

            // Calculate growth metrics
            var previousPeriodStart = startDate.AddDays(-(endDate - startDate).Days);
            var previousPeriodEnd = startDate.AddDays(-1);
            var previousReport = await _reportingService.GenerateSalesReportAsync(previousPeriodStart, previousPeriodEnd, branchId, warehouseId, cancellationToken);

            var salesGrowth = previousReport.TotalSales > 0 
                ? ((salesReport.TotalSales - previousReport.TotalSales) / previousReport.TotalSales) * 100m 
                : 0m;

            var transactionGrowth = previousReport.TotalTransactions > 0 
                ? ((double)(salesReport.TotalTransactions - previousReport.TotalTransactions) / previousReport.TotalTransactions) * 100 
                : 0;

            // Calculate period-based analytics
            var periodData = CalculatePeriodAnalytics(salesReport.DailySales, period);

            // Calculate seasonal trends
            var seasonalTrends = CalculateSeasonalTrends(salesReport.DailySales);

            // Calculate product performance metrics
            var productPerformance = CalculateProductPerformance(salesReport.TopProducts);

            // Calculate customer behavior patterns
            var customerBehavior = await CalculateCustomerBehavior(startDate, endDate, cancellationToken);

            return new SalesAnalytics
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period,
                TotalSales = salesReport.TotalSales,
                SalesGrowth = salesGrowth,
                TotalTransactions = salesReport.TotalTransactions,
                TransactionGrowth = transactionGrowth,
                AverageTransactionValue = salesReport.AverageTransactionValue,
                PeriodData = periodData,
                SeasonalTrends = seasonalTrends,
                TopProducts = productPerformance,
                CustomerBehavior = customerBehavior,
                SalesByPaymentMethod = salesReport.SalesByPaymentMethod,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales analytics from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<InventoryAnalytics> GetInventoryAnalyticsAsync(
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating inventory analytics for branch {BranchId} and warehouse {WarehouseId}", branchId, warehouseId);

            var inventoryReport = await _reportingService.GenerateInventoryReportAsync(branchId, warehouseId, cancellationToken);

            // Calculate inventory turnover
            var inventoryTurnover = await CalculateInventoryTurnover(branchId, warehouseId, cancellationToken);

            // Calculate stock health metrics
            var stockHealth = CalculateStockHealth(inventoryReport);

            // Calculate category performance
            var categoryPerformance = CalculateCategoryPerformance(inventoryReport.StockLevelsByCategory);

            // Calculate movement patterns
            var movementPatterns = await CalculateMovementPatterns(branchId, warehouseId, cancellationToken);

            // Calculate demand forecasting
            var demandForecast = await CalculateDemandForecast(branchId, warehouseId, cancellationToken);

            return new InventoryAnalytics
            {
                BranchId = branchId,
                WarehouseId = warehouseId,
                TotalItems = inventoryReport.TotalItems,
                TotalValue = inventoryReport.TotalValue,
                InventoryTurnover = inventoryTurnover,
                StockHealth = stockHealth,
                CategoryPerformance = categoryPerformance,
                MovementPatterns = movementPatterns,
                DemandForecast = demandForecast,
                LowStockAlerts = inventoryReport.LowStockAlerts.Count,
                OutOfStockItems = inventoryReport.OutOfStockItems,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating inventory analytics for branch {BranchId} and warehouse {WarehouseId}", branchId, warehouseId);
            throw;
        }
    }

    public async Task<CustomerAnalytics> GetCustomerAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating customer analytics from {StartDate} to {EndDate}", startDate, endDate);

            var customerReport = await _reportingService.GenerateCustomerReportAsync(startDate, endDate, cancellationToken);

            // Calculate customer acquisition metrics
            var acquisitionMetrics = await CalculateCustomerAcquisition(startDate, endDate, cancellationToken);

            // Calculate customer retention metrics
            var retentionMetrics = await CalculateCustomerRetention(startDate, endDate, cancellationToken);

            // Calculate customer lifetime value
            var lifetimeValue = await CalculateCustomerLifetimeValue(cancellationToken);

            // Calculate customer segmentation analytics
            var segmentationAnalytics = await CalculateSegmentationAnalytics(cancellationToken);

            // Calculate loyalty program effectiveness
            var loyaltyEffectiveness = await CalculateLoyaltyEffectiveness(cancellationToken);

            return new CustomerAnalytics
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalCustomers = customerReport.TotalCustomers,
                ActiveCustomers = customerReport.ActiveCustomers,
                NewCustomers = customerReport.NewCustomers,
                AcquisitionMetrics = acquisitionMetrics,
                RetentionMetrics = retentionMetrics,
                CustomerLifetimeValue = lifetimeValue,
                SegmentationAnalytics = segmentationAnalytics,
                LoyaltyEffectiveness = loyaltyEffectiveness,
                TopCustomers = customerReport.TopCustomers,
                CustomerSegments = customerReport.CustomerSegments,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating customer analytics from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<FinancialAnalytics> GetFinancialAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        AnalyticsPeriod period = AnalyticsPeriod.Monthly,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating financial analytics from {StartDate} to {EndDate}", startDate, endDate);

            var financialReport = await _reportingService.GenerateFinancialReportAsync(startDate, endDate, cancellationToken);

            // Calculate profitability metrics
            var profitabilityMetrics = await CalculateProfitabilityMetrics(startDate, endDate, cancellationToken);

            // Calculate cash flow analytics
            var cashFlowAnalytics = await CalculateCashFlowAnalytics(startDate, endDate, cancellationToken);

            // Calculate expense breakdown
            var expenseBreakdown = await CalculateExpenseBreakdown(startDate, endDate, cancellationToken);

            // Calculate revenue trends
            var revenueTrends = await CalculateRevenueTrends(startDate, endDate, period, cancellationToken);

            // Calculate financial ratios
            var financialRatios = CalculateFinancialRatios(financialReport);

            return new FinancialAnalytics
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period,
                TotalRevenue = financialReport.TotalRevenue,
                TotalCosts = financialReport.TotalCosts,
                GrossProfit = financialReport.GrossProfit,
                ProfitMargin = financialReport.ProfitMargin,
                ProfitabilityMetrics = profitabilityMetrics,
                CashFlowAnalytics = cashFlowAnalytics,
                ExpenseBreakdown = expenseBreakdown,
                RevenueTrends = revenueTrends,
                FinancialRatios = financialRatios,
                MonthlyBreakdown = financialReport.MonthlyBreakdown,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial analytics from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<PredictiveAnalytics> GetPredictiveAnalyticsAsync(
        DateTime startDate,
        DateTime endDate,
        PredictionType predictionType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating predictive analytics from {StartDate} to {EndDate} for {PredictionType}", startDate, endDate, predictionType);

            switch (predictionType)
            {
                case PredictionType.Sales:
                    return await GenerateSalesPrediction(startDate, endDate, cancellationToken);
                
                case PredictionType.Inventory:
                    return await GenerateInventoryPrediction(startDate, endDate, cancellationToken);
                
                case PredictionType.Customer:
                    return await GenerateCustomerPrediction(startDate, endDate, cancellationToken);
                
                case PredictionType.Financial:
                    return await GenerateFinancialPrediction(startDate, endDate, cancellationToken);
                
                default:
                    throw new ArgumentException($"Unsupported prediction type: {predictionType}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating predictive analytics for {PredictionType}", predictionType);
            throw;
        }
    }

    #region Helper Methods

    private List<PeriodData> CalculatePeriodAnalytics(List<DailySalesData> dailySales, AnalyticsPeriod period)
    {
        var periodData = new List<PeriodData>();

        switch (period)
        {
            case AnalyticsPeriod.Daily:
                periodData = dailySales.Select(d => new PeriodData
                {
                    Period = d.Date.ToString("yyyy-MM-dd"),
                    Value = d.TotalSales,
                    Count = d.TransactionCount
                }).ToList();
                break;
            
            case AnalyticsPeriod.Weekly:
                var weeklyData = dailySales.GroupBy(d => GetWeekNumber(d.Date));
                periodData = weeklyData.Select(g => new PeriodData
                {
                    Period = $"Week {g.Key}",
                    Value = g.Sum(d => d.TotalSales),
                    Count = g.Sum(d => d.TransactionCount)
                }).ToList();
                break;
            
            case AnalyticsPeriod.Monthly:
                var monthlyData = dailySales.GroupBy(d => new { d.Date.Year, d.Date.Month });
                periodData = monthlyData.Select(g => new PeriodData
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Value = g.Sum(d => d.TotalSales),
                    Count = g.Sum(d => d.TransactionCount)
                }).ToList();
                break;
        }

        return periodData;
    }

    private List<SeasonalTrend> CalculateSeasonalTrends(List<DailySalesData> dailySales)
    {
        // Simple seasonal trend analysis based on day of week and month
        var dayOfWeekTrends = dailySales
            .GroupBy(d => d.Date.DayOfWeek)
            .Select(g => new SeasonalTrend
            {
                Period = g.Key.ToString(),
                AverageValue = g.Average(d => d.TotalSales),
                Trend = CalculateTrend(g.Select(d => d.TotalSales).ToList())
            })
            .ToList();

        return dayOfWeekTrends;
    }

    private List<ProductPerformance> CalculateProductPerformance(List<ProductSalesData> topProducts)
    {
        return topProducts.Select(p => new ProductPerformance
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            TotalRevenue = p.TotalRevenue,
            TotalQuantity = p.TotalQuantity,
            AveragePrice = p.AveragePrice,
            PerformanceScore = CalculatePerformanceScore(p.TotalRevenue, p.TotalQuantity)
        }).ToList();
    }

    private async Task<CustomerBehavior> CalculateCustomerBehavior(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        // This would analyze customer purchasing patterns
        return new CustomerBehavior
        {
            AveragePurchaseFrequency = 2.5, // Example calculation
            AveragePurchaseValue = 150.00m, // Example calculation
            PeakShoppingHours = new List<int> { 10, 14, 18 }, // Example data
            PreferredPaymentMethods = new List<string> { "Credit Card", "Cash" } // Example data
        };
    }

    private async Task<double> CalculateInventoryTurnover(Guid? branchId, Guid? warehouseId, CancellationToken cancellationToken)
    {
        // Calculate inventory turnover ratio
        // This would typically be: Cost of Goods Sold / Average Inventory
        return 4.5; // Example value
    }

    private StockHealth CalculateStockHealth(InventoryReport inventoryReport)
    {
        var totalItems = inventoryReport.TotalItems;
        var healthyItems = totalItems - inventoryReport.LowStockItems - inventoryReport.OutOfStockItems;
        var healthScore = totalItems > 0 ? (healthyItems * 100.0 / totalItems) : 0;

        return new StockHealth
        {
            HealthScore = healthScore,
            HealthyItems = healthyItems,
            LowStockItems = inventoryReport.LowStockItems,
            OutOfStockItems = inventoryReport.OutOfStockItems,
            Status = healthScore >= 80 ? "Excellent" : healthScore >= 60 ? "Good" : healthScore >= 40 ? "Fair" : "Poor"
        };
    }

    private List<CategoryPerformance> CalculateCategoryPerformance(List<CategoryStockData> stockLevels)
    {
        return stockLevels.Select(c => new CategoryPerformance
        {
            Category = c.Category,
            TotalValue = c.TotalValue,
            ItemCount = c.TotalItems,
            PerformanceScore = CalculateCategoryPerformanceScore(c.TotalValue, c.TotalItems, c.LowStockItems)
        }).OrderByDescending(cp => cp.PerformanceScore).ToList();
    }

    private async Task<MovementPattern> CalculateMovementPatterns(Guid? branchId, Guid? warehouseId, CancellationToken cancellationToken)
    {
        // Analyze inventory movement patterns
        return new MovementPattern
        {
            AverageMovementPerDay = 150, // Example
            PeakMovementDays = new List<string> { "Monday", "Friday" }, // Example
            MovementTrend = "Increasing", // Example
            Seasonality = "High" // Example
        };
    }

    private async Task<DemandForecast> CalculateDemandForecast(Guid? branchId, Guid? warehouseId, CancellationToken cancellationToken)
    {
        // Generate demand forecast based on historical data
        return new DemandForecast
        {
            NextPeriodDemand = 1250, // Example
            ConfidenceLevel = 85, // Example
            ForecastAccuracy = 92, // Example
            RecommendedReorderLevel = 800 // Example
        };
    }

    private async Task<CustomerAcquisition> CalculateCustomerAcquisition(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new CustomerAcquisition
        {
            NewCustomersCount = 45, // Example
            AcquisitionCost = 25.50m, // Example
            AcquisitionChannels = new List<AcquisitionChannel>
            {
                new() { Channel = "Online", Customers = 25, Percentage = 55.6 },
                new() { Channel = "In-Store", Customers = 20, Percentage = 44.4 }
            }
        };
    }

    private async Task<CustomerRetention> CalculateCustomerRetention(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new CustomerRetention
        {
            RetentionRate = 78.5, // Example
            ChurnRate = 21.5, // Example
            AverageCustomerLifetime = 18, // Example in months
            RepeatPurchaseRate = 65.2 // Example
        };
    }

    private async Task<CustomerLifetimeValue> CalculateCustomerLifetimeValue(CancellationToken cancellationToken)
    {
        return new CustomerLifetimeValue
        {
            AverageLifetimeValue = 1250.00m, // Example
            AveragePurchaseValue = 85.50m, // Example
            PurchaseFrequency = 3.2, // Example per month
            AverageCustomerLifespan = 24 // Example in months
        };
    }

    private async Task<SegmentationAnalytics> CalculateSegmentationAnalytics(CancellationToken cancellationToken)
    {
        return new SegmentationAnalytics
        {
            HighValueCustomers = 125, // Example
            MediumValueCustomers = 350, // Example
            LowValueCustomers = 525, // Example
            SegmentDistribution = new List<SegmentDistribution>
            {
                new() { Segment = "VIP", Count = 50, Percentage = 5.0 },
                new() { Segment = "Regular", Count = 600, Percentage = 60.0 },
                new() { Segment = "Occasional", Count = 350, Percentage = 35.0 }
            }
        };
    }

    private async Task<LoyaltyEffectiveness> CalculateLoyaltyEffectiveness(CancellationToken cancellationToken)
    {
        return new LoyaltyEffectiveness
        {
            ProgramParticipationRate = 68.5, // Example
            PointsRedemptionRate = 42.3, // Example
            LoyaltyMemberSpendIncrease = 23.5, // Example percentage
            ProgramROI = 145.6 // Example percentage
        };
    }

    private async Task<ProfitabilityMetrics> CalculateProfitabilityMetrics(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new ProfitabilityMetrics
        {
            GrossProfitMargin = 35.2, // Example
            NetProfitMargin = 12.8, // Example
            OperatingMargin = 18.5, // Example
            ReturnOnAssets = 15.3 // Example
        };
    }

    private async Task<CashFlowAnalytics> CalculateCashFlowAnalytics(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new CashFlowAnalytics
        {
            OperatingCashFlow = 45000.00m, // Example
            InvestingCashFlow = -15000.00m, // Example
            FinancingCashFlow = 5000.00m, // Example
            FreeCashFlow = 30000.00m // Example
        };
    }

    private async Task<ExpenseBreakdown> CalculateExpenseBreakdown(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new ExpenseBreakdown
        {
            CostOfGoodsSold = 65000.00m, // Example
            OperatingExpenses = 25000.00m, // Example
            MarketingExpenses = 8000.00m, // Example
            AdministrativeExpenses = 12000.00m// Example
        };
    }

    private async Task<RevenueTrends> CalculateRevenueTrends(DateTime startDate, DateTime endDate, AnalyticsPeriod period, CancellationToken cancellationToken)
    {
        return new RevenueTrends
        {
            CurrentPeriodRevenue = 125000.00m, // Example
            PreviousPeriodRevenue = 115000.00m, // Example
            GrowthRate = 8.7, // Example percentage
            TrendDirection = "Upward", // Example
            SeasonalImpact = "Positive" // Example
        };
    }

    private FinancialRatios CalculateFinancialRatios(FinancialReport financialReport)
    {
        return new FinancialRatios
        {
            CurrentRatio = 2.1, // Example
            QuickRatio = 1.8, // Example
            DebtToEquityRatio = 0.6, // Example
            InventoryTurnoverRatio = 4.5 // Example
        };
    }

    private async Task<PredictiveAnalytics> GenerateSalesPrediction(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new PredictiveAnalytics
        {
            PredictionType = PredictionType.Sales,
            PredictedValue = 135000.00m, // Example
            ConfidenceLevel = 87, // Example
            PredictionPeriod = "Next 30 days",
            Factors = new List<string> { "Seasonal trends", "Historical growth", "Market conditions" }
        };
    }

    private async Task<PredictiveAnalytics> GenerateInventoryPrediction(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new PredictiveAnalytics
        {
            PredictionType = PredictionType.Inventory,
            PredictedValue = 2500, // Example quantity
            ConfidenceLevel = 82, // Example
            PredictionPeriod = "Next 30 days",
            Factors = new List<string> { "Demand forecast", "Lead time", "Safety stock requirements" }
        };
    }

    private async Task<PredictiveAnalytics> GenerateCustomerPrediction(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new PredictiveAnalytics
        {
            PredictionType = PredictionType.Customer,
            PredictedValue = 55, // Example new customers
            ConfidenceLevel = 79, // Example
            PredictionPeriod = "Next 30 days",
            Factors = new List<string> { "Marketing campaigns", "Seasonal demand", "Economic indicators" }
        };
    }

    private async Task<PredictiveAnalytics> GenerateFinancialPrediction(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return new PredictiveAnalytics
        {
            PredictionType = PredictionType.Financial,
            PredictedValue = 15000.00m, // Example profit
            ConfidenceLevel = 75, // Example
            PredictionPeriod = "Next 30 days",
            Factors = new List<string> { "Revenue projections", "Cost trends", "Market conditions" }
        };
    }

    #endregion

    #region Utility Methods

    private int GetWeekNumber(DateTime date)
    {
        return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    private string CalculateTrend(List<decimal> values)
    {
        if (values.Count < 2) return "Insufficient data";

        var firstHalf = values.Take(values.Count / 2).Average();
        var secondHalf = values.Skip(values.Count / 2).Average();

        if (secondHalf > firstHalf * 1.05m) return "Increasing";
        if (secondHalf < firstHalf * 0.95m) return "Decreasing";
        return "Stable";
    }

    private double CalculatePerformanceScore(decimal revenue, int quantity)
    {
        // Simple performance score calculation
        return (double)(revenue * 0.7m + quantity * 0.3m);
    }

    private double CalculateCategoryPerformanceScore(decimal value, int itemCount, int lowStockItems)
    {
        // Category performance based on value, item count, and stock health
        var valueScore = (double)value / 1000; // Normalize value
        var stockHealthScore = itemCount > 0 ? ((itemCount - lowStockItems) * 100.0 / itemCount) : 0;
        return (valueScore * 0.6 + stockHealthScore * 0.4);
    }

    #endregion
}

// Enums and data classes for analytics
public enum AnalyticsPeriod
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public enum PredictionType
{
    Sales,
    Inventory,
    Customer,
    Financial
}

// Main analytics data classes
public class SalesAnalytics
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AnalyticsPeriod Period { get; set; }
    public decimal TotalSales { get; set; }
    public decimal SalesGrowth { get; set; }
    public int TotalTransactions { get; set; }
    public double TransactionGrowth { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public List<PeriodData> PeriodData { get; set; } = new();
    public List<SeasonalTrend> SeasonalTrends { get; set; } = new();
    public List<ProductPerformance> TopProducts { get; set; } = new();
    public CustomerBehavior CustomerBehavior { get; set; } = new();
    public List<PaymentMethodData> SalesByPaymentMethod { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class InventoryAnalytics
{
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public double InventoryTurnover { get; set; }
    public StockHealth StockHealth { get; set; } = new();
    public List<CategoryPerformance> CategoryPerformance { get; set; } = new();
    public MovementPattern MovementPatterns { get; set; } = new();
    public DemandForecast DemandForecast { get; set; } = new();
    public int LowStockAlerts { get; set; }
    public int OutOfStockItems { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class CustomerAnalytics
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomers { get; set; }
    public CustomerAcquisition AcquisitionMetrics { get; set; } = new();
    public CustomerRetention RetentionMetrics { get; set; } = new();
    public CustomerLifetimeValue CustomerLifetimeValue { get; set; } = new();
    public SegmentationAnalytics SegmentationAnalytics { get; set; } = new();
    public LoyaltyEffectiveness LoyaltyEffectiveness { get; set; } = new();
    public List<TopCustomerData> TopCustomers { get; set; } = new();
    public List<CustomerSegmentData> CustomerSegments { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class FinancialAnalytics
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AnalyticsPeriod Period { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public ProfitabilityMetrics ProfitabilityMetrics { get; set; } = new();
    public CashFlowAnalytics CashFlowAnalytics { get; set; } = new();
    public ExpenseBreakdown ExpenseBreakdown { get; set; } = new();
    public RevenueTrends RevenueTrends { get; set; } = new();
    public FinancialRatios FinancialRatios { get; set; } = new();
    public List<MonthlyFinancialData> MonthlyBreakdown { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class PredictiveAnalytics
{
    public PredictionType PredictionType { get; set; }
    public decimal PredictedValue { get; set; }
    public int ConfidenceLevel { get; set; }
    public string PredictionPeriod { get; set; } = string.Empty;
    public List<string> Factors { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

// Supporting data classes
public class PeriodData
{
    public string Period { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int Count { get; set; }
}

public class SeasonalTrend
{
    public string Period { get; set; } = string.Empty;
    public decimal AverageValue { get; set; }
    public string Trend { get; set; } = string.Empty;
}

public class ProductPerformance
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalQuantity { get; set; }
    public decimal AveragePrice { get; set; }
    public double PerformanceScore { get; set; }
}

public class CustomerBehavior
{
    public double AveragePurchaseFrequency { get; set; }
    public decimal AveragePurchaseValue { get; set; }
    public List<int> PeakShoppingHours { get; set; } = new();
    public List<string> PreferredPaymentMethods { get; set; } = new();
}

public class StockHealth
{
    public double HealthScore { get; set; }
    public int HealthyItems { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CategoryPerformance
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public int ItemCount { get; set; }
    public double PerformanceScore { get; set; }
}

public class MovementPattern
{
    public double AverageMovementPerDay { get; set; }
    public List<string> PeakMovementDays { get; set; } = new();
    public string MovementTrend { get; set; } = string.Empty;
    public string Seasonality { get; set; } = string.Empty;
}

public class DemandForecast
{
    public int NextPeriodDemand { get; set; }
    public int ConfidenceLevel { get; set; }
    public int ForecastAccuracy { get; set; }
    public int RecommendedReorderLevel { get; set; }
}

public class CustomerAcquisition
{
    public int NewCustomersCount { get; set; }
    public decimal AcquisitionCost { get; set; }
    public List<AcquisitionChannel> AcquisitionChannels { get; set; } = new();
}

public class AcquisitionChannel
{
    public string Channel { get; set; } = string.Empty;
    public int Customers { get; set; }
    public double Percentage { get; set; }
}

public class CustomerRetention
{
    public double RetentionRate { get; set; }
    public double ChurnRate { get; set; }
    public double AverageCustomerLifetime { get; set; }
    public double RepeatPurchaseRate { get; set; }
}

public class CustomerLifetimeValue
{
    public decimal AverageLifetimeValue { get; set; }
    public decimal AveragePurchaseValue { get; set; }
    public double PurchaseFrequency { get; set; }
    public double AverageCustomerLifespan { get; set; }
}

public class SegmentationAnalytics
{
    public int HighValueCustomers { get; set; }
    public int MediumValueCustomers { get; set; }
    public int LowValueCustomers { get; set; }
    public List<SegmentDistribution> SegmentDistribution { get; set; } = new();
}

public class SegmentDistribution
{
    public string Segment { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class LoyaltyEffectiveness
{
    public double ProgramParticipationRate { get; set; }
    public double PointsRedemptionRate { get; set; }
    public double LoyaltyMemberSpendIncrease { get; set; }
    public double ProgramROI { get; set; }
}

public class ProfitabilityMetrics
{
    public double GrossProfitMargin { get; set; }
    public double NetProfitMargin { get; set; }
    public double OperatingMargin { get; set; }
    public double ReturnOnAssets { get; set; }
}

public class CashFlowAnalytics
{
    public decimal OperatingCashFlow { get; set; }
    public decimal InvestingCashFlow { get; set; }
    public decimal FinancingCashFlow { get; set; }
    public decimal FreeCashFlow { get; set; }
}

public class ExpenseBreakdown
{
    public decimal CostOfGoodsSold { get; set; }
    public decimal OperatingExpenses { get; set; }
    public decimal MarketingExpenses { get; set; }
    public decimal AdministrativeExpenses { get; set; }
}

public class RevenueTrends
{
    public decimal CurrentPeriodRevenue { get; set; }
    public decimal PreviousPeriodRevenue { get; set; }
    public double GrowthRate { get; set; }
    public string TrendDirection { get; set; } = string.Empty;
    public string SeasonalImpact { get; set; } = string.Empty;
}

public class FinancialRatios
{
    public double CurrentRatio { get; set; }
    public double QuickRatio { get; set; }
    public double DebtToEquityRatio { get; set; }
    public double InventoryTurnoverRatio { get; set; }
}
