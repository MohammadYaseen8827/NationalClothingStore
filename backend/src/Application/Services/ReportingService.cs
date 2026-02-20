using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service for generating various business reports
/// </summary>
public class ReportingService : IReportingService
{
    private readonly IInventoryManagementService _inventoryManagementService;
    private readonly ISalesProcessingService _salesProcessingService;
    private readonly ICustomerManagementService _customerManagementService;
    private readonly IProcurementManagementService _procurementManagementService;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(
        IInventoryManagementService inventoryManagementService,
        ISalesProcessingService salesProcessingService,
        ICustomerManagementService customerManagementService,
        IProcurementManagementService procurementManagementService,
        ILogger<ReportingService> logger)
    {
        _inventoryManagementService = inventoryManagementService;
        _salesProcessingService = salesProcessingService;
        _customerManagementService = customerManagementService;
        _procurementManagementService = procurementManagementService;
        _logger = logger;
    }

    public async Task<SalesReport> GenerateSalesReportAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating sales report from {StartDate} to {EndDate}", startDate, endDate);

            // Get sales transactions for the period
            var salesTransactions = await _salesProcessingService.GetSalesTransactionsAsync(
                startDate, endDate, branchId, warehouseId, cancellationToken);

            // Calculate metrics
            var totalSales = salesTransactions.Sum(t => t.TotalAmount);
            var totalTransactions = salesTransactions.Count();
            var averageTransactionValue = totalTransactions > 0 ? totalSales / totalTransactions : 0;
            
            // Group by day for trend analysis
            var dailySales = salesTransactions
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new DailySalesData
                {
                    Date = g.Key,
                    TotalSales = g.Sum(t => t.TotalAmount),
                    TransactionCount = g.Count(),
                    AverageTransactionValue = g.Count() > 0 ? g.Sum(t => t.TotalAmount) / g.Count() : 0
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Top selling products
            var topProducts = salesTransactions
                .SelectMany(t => t.Items)
                .GroupBy(i => i.ProductId)
                .Select(g => new ProductSalesData
                {
                    ProductId = g.Key,
                    ProductName = g.First().ProductName,
                    TotalQuantity = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.TotalPrice),
                    AveragePrice = g.Sum(i => i.TotalPrice) / g.Sum(i => i.Quantity)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(10)
                .ToList();

            // Sales by payment method
            var salesByPaymentMethod = salesTransactions
                .SelectMany(t => t.Payments)
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new PaymentMethodData
                {
                    PaymentMethod = g.Key,
                    TotalAmount = g.Sum(p => p.Amount),
                    TransactionCount = g.Count(),
                    Percentage = totalSales > 0 ? (g.Sum(p => p.Amount) / totalSales) * 100 : 0
                })
                .OrderByDescending(p => p.TotalAmount)
                .ToList();

            return new SalesReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = totalSales,
                TotalTransactions = totalTransactions,
                AverageTransactionValue = averageTransactionValue,
                DailySales = dailySales,
                TopProducts = topProducts,
                SalesByPaymentMethod = salesByPaymentMethod,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales report from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<InventoryReport> GenerateInventoryReportAsync(
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating inventory report for branch {BranchId} and warehouse {WarehouseId}", branchId, warehouseId);

            // Get inventory summary
            var inventorySummary = await _inventoryManagementService.GetInventorySummaryAsync(branchId, warehouseId, cancellationToken);

            // Get low stock alerts
            var lowStockAlerts = await _inventoryManagementService.GetLowStockAlertsAsync(branchId, warehouseId, cancellationToken);

            // Get inventory movements
            var recentMovements = await _inventoryManagementService.GetRecentMovementsAsync(
                DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, branchId, warehouseId, cancellationToken);

            // Calculate inventory value
            var totalInventoryValue = inventorySummary.TotalItems * inventorySummary.AverageUnitCost;

            // Stock levels by category
            var stockLevelsByCategory = inventorySummary.InventoryByCategory
                .Select(c => new CategoryStockData
                {
                    Category = c.Category,
                    TotalItems = c.TotalItems,
                    TotalValue = c.TotalItems * c.AverageUnitCost,
                    LowStockItems = c.LowStockItems,
                    OutOfStockItems = c.OutOfStockItems
                })
                .OrderByDescending(c => c.TotalValue)
                .ToList();

            return new InventoryReport
            {
                BranchId = branchId,
                WarehouseId = warehouseId,
                TotalItems = inventorySummary.TotalItems,
                TotalValue = totalInventoryValue,
                AverageUnitCost = inventorySummary.AverageUnitCost,
                LowStockItems = inventorySummary.LowStockItems,
                OutOfStockItems = inventorySummary.OutOfStockItems,
                StockLevelsByCategory = stockLevelsByCategory,
                LowStockAlerts = lowStockAlerts.ToList(),
                RecentMovements = recentMovements,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating inventory report for branch {BranchId} and warehouse {WarehouseId}", branchId, warehouseId);
            throw;
        }
    }

    public async Task<CustomerReport> GenerateCustomerReportAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating customer report from {StartDate} to {EndDate}", startDate, endDate);

            // Get all customers
            var customers = (await _customerManagementService.GetCustomersAsync(1, 1000, cancellationToken)).ToList();

            // Calculate customer metrics
            var totalCustomers = customers.Count;
            var activeCustomers = customers.Count(c => c.IsActive);
            var newCustomers = customers.Count(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate);

            // Customer loyalty metrics
            var loyaltyAccounts = new List<CustomerLoyalty>();
            foreach (var customer in customers)
            {
                var loyalty = await _customerManagementService.GetCustomerLoyaltyAsync(customer.Id, cancellationToken);
                if (loyalty != null)
                {
                    loyaltyAccounts.Add(loyalty);
                }
            }

            var totalLoyaltyPoints = loyaltyAccounts.Sum(l => l.AvailablePoints);
            var averageLoyaltyPoints = loyaltyAccounts.Count > 0 ? totalLoyaltyPoints / loyaltyAccounts.Count : 0;

            // Customer segments
            var customerSegments = new List<CustomerSegmentData>
            {
                new() { Segment = "New", Count = newCustomers, Percentage = totalCustomers > 0 ? (newCustomers * 100.0 / totalCustomers) : 0 },
                new() { Segment = "Active", Count = activeCustomers, Percentage = totalCustomers > 0 ? (activeCustomers * 100.0 / totalCustomers) : 0 },
                new() { Segment = "Inactive", Count = totalCustomers - activeCustomers, Percentage = totalCustomers > 0 ? ((totalCustomers - activeCustomers) * 100.0 / totalCustomers) : 0 }
            };

            // Top customers by loyalty points
            var topCustomers = loyaltyAccounts
                .OrderByDescending(l => l.AvailablePoints)
                .Take(10)
                .Select(l => new TopCustomerData
                {
                    CustomerId = l.CustomerId,
                    CustomerName = customers.FirstOrDefault(c => c.Id == l.CustomerId)?.ToString() ?? "Unknown",
                    LoyaltyPoints = l.AvailablePoints,
                    Tier = l.Tier
                })
                .ToList();

            return new CustomerReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalCustomers = totalCustomers,
                ActiveCustomers = activeCustomers,
                NewCustomers = newCustomers,
                TotalLoyaltyPoints = totalLoyaltyPoints,
                AverageLoyaltyPoints = averageLoyaltyPoints,
                CustomerSegments = customerSegments,
                TopCustomers = topCustomers,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating customer report from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<ProcurementReport> GenerateProcurementReportAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating procurement report from {StartDate} to {EndDate}", startDate, endDate);

            // Get purchase orders for the period
            var purchaseOrders = await _procurementManagementService.GetPurchaseOrdersAsync(1, 1000, cancellationToken);
            var filteredOrders = purchaseOrders
                .Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate)
                .ToList();

            // Calculate procurement metrics
            var totalPurchaseValue = filteredOrders.Sum(po => po.TotalAmount);
            var totalOrders = filteredOrders.Count();
            var averageOrderValue = totalOrders > 0 ? totalPurchaseValue / totalOrders : 0;

            // Orders by status
            var ordersByStatus = filteredOrders
                .GroupBy(po => po.Status)
                .Select(g => new OrderStatusData
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(po => po.TotalAmount),
                    Percentage = totalOrders > 0 ? (g.Count() * 100.0 / totalOrders) : 0
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            // Top suppliers by order value
            var topSuppliers = filteredOrders
                .GroupBy(po => po.SupplierId)
                .Select(g => new SupplierProcurementData
                {
                    SupplierId = g.Key,
                    SupplierName = "Supplier " + g.Key.ToString()[..8], // Would get actual name from supplier service
                    TotalOrders = g.Count(),
                    TotalValue = g.Sum(po => po.TotalAmount),
                    AverageOrderValue = g.Count() > 0 ? g.Sum(po => po.TotalAmount) / g.Count() : 0
                })
                .OrderByDescending(s => s.TotalValue)
                .Take(10)
                .ToList();

            return new ProcurementReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalPurchaseValue = totalPurchaseValue,
                TotalOrders = totalOrders,
                AverageOrderValue = averageOrderValue,
                OrdersByStatus = ordersByStatus,
                TopSuppliers = topSuppliers,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating procurement report from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<FinancialReport> GenerateFinancialReportAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating financial report from {StartDate} to {EndDate}", startDate, endDate);

            // Get sales data
            var salesReport = await GenerateSalesReportAsync(startDate, endDate, null, null, cancellationToken);

            // Get procurement data
            var procurementReport = await GenerateProcurementReportAsync(startDate, endDate, cancellationToken);

            // Calculate financial metrics
            var totalRevenue = salesReport.TotalSales;
            var totalCosts = procurementReport.TotalPurchaseValue;
            var grossProfit = totalRevenue - totalCosts;
            var profitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0;

            // Monthly breakdown
            var monthlyData = Enumerable.Range(0, ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1)
                .Select(i => startDate.AddMonths(i))
                .Select(month => new MonthlyFinancialData
                {
                    Month = month,
                    Revenue = 0, // Would calculate from actual sales data
                    Costs = 0, // Would calculate from actual procurement data
                    Profit = 0
                })
                .ToList();

            return new FinancialReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue,
                TotalCosts = totalCosts,
                GrossProfit = grossProfit,
                ProfitMargin = profitMargin,
                MonthlyBreakdown = monthlyData,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial report from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }
}

// Data transfer objects for reports
public class SalesReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public List<DailySalesData> DailySales { get; set; } = new();
    public List<ProductSalesData> TopProducts { get; set; } = new();
    public List<PaymentMethodData> SalesByPaymentMethod { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class InventoryReport
{
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageUnitCost { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public List<CategoryStockData> StockLevelsByCategory { get; set; } = new();
    public List<LowStockAlert> LowStockAlerts { get; set; } = new();
    public List<InventoryMovement> RecentMovements { get; set; } = new();
         public InventoryStatistics Statistics { get; init; } = new();
     public List<LocationInventoryValue> LocationValues { get; init; } = new();
     public List<TransactionSummary> TransactionSummaries { get; init; } = new();
     public DateTime ReportDate { get; init; }
     public string GeneratedBy { get; init; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class CustomerReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomers { get; set; }
    public int TotalLoyaltyPoints { get; set; }
    public double AverageLoyaltyPoints { get; set; }
    public List<CustomerSegmentData> CustomerSegments { get; set; } = new();
    public List<TopCustomerData> TopCustomers { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class ProcurementReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPurchaseValue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<OrderStatusData> OrdersByStatus { get; set; } = new();
    public List<SupplierProcurementData> TopSuppliers { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class FinancialReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public List<MonthlyFinancialData> MonthlyBreakdown { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

// Supporting data classes
public class DailySalesData
{
    public DateTime Date { get; set; }
    public decimal TotalSales { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTransactionValue { get; set; }
}

public class ProductSalesData
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePrice { get; set; }
}

public class PaymentMethodData
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public double Percentage { get; set; }
}

public class CategoryStockData
{
    public string Category { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
}

public class CustomerSegmentData
{
    public string Segment { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class TopCustomerData
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int LoyaltyPoints { get; set; }
    public string Tier { get; set; } = string.Empty;
}

public class OrderStatusData
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public double Percentage { get; set; }
}

public class SupplierProcurementData
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class MonthlyFinancialData
{
    public DateTime Month { get; set; }
    public decimal Revenue { get; set; }
    public decimal Costs { get; set; }
    public decimal Profit { get; set; }
}
