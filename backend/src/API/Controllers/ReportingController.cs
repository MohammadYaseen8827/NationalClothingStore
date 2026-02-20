using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Validation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;

namespace NationalClothingStore.API.Controllers;

// Role-based authorization policy names
public static class ReportingPolicies
{
    public const string ViewReports = "Reporting.View";
    public const string ViewAnalytics = "Reporting.Analytics";
    public const string ExportReports = "Reporting.Export";
    public const string ManageReports = "Reporting.Manage";
}

// Performance metrics tracking
public class ReportingMetrics
{
    public string Endpoint { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public long TotalElapsedMs { get; set; }
    public double AverageMs => TotalRequests > 0 ? (double)TotalElapsedMs / TotalRequests : 0;
    public int Errors { get; set; }
    public DateTime LastAccessed { get; set; }
}

/// <summary>
/// Controller for reporting and analytics operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportingController : ControllerBase
{
    private readonly IReportingService _reportingService;
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<ReportingController> _logger;
    private readonly IMemoryCache _cache;
    private static readonly ConcurrentDictionary<string, ReportingMetrics> _metrics = new();

    public ReportingController(
        IReportingService reportingService,
        IAnalyticsService analyticsService,
        ILogger<ReportingController> logger,
        IMemoryCache cache)
    {
        _reportingService = reportingService;
        _analyticsService = analyticsService;
        _logger = logger;
        _cache = cache;
    }

    private IActionResult ValidationError(string message) => 
        BadRequest(new { error = message, code = "validation_error" });

    private IActionResult ServerError(string message) => 
        StatusCode(500, new { error = message, code = "server_error" });

    private static void ValidateDateRange(DateTime? start, DateTime? end)
    {
        if (!start.HasValue || !end.HasValue)
            throw new InvalidOperationException("Both StartDate and EndDate are required.");
        if (start > end)
            throw new InvalidOperationException("StartDate cannot be greater than EndDate.");
    }

    private void RequirePolicy(string policy)
    {
        if (!User.HasClaim(c => c.Type == "permission" && c.Value == policy))
        {
            _logger.LogWarning("Access denied: User {UserId} lacks policy {Policy}", User.Identity?.Name ?? "Unknown", policy);
            throw new UnauthorizedAccessException($"Access denied. Required policy: {policy}");
        }
    }

    private void RecordMetric(string endpoint, long elapsedMs, bool isError)
    {
        var metric = _metrics.AddOrUpdate(endpoint,
            new ReportingMetrics { Endpoint = endpoint, TotalRequests = 1, TotalElapsedMs = elapsedMs, Errors = isError ? 1 : 0, LastAccessed = DateTime.UtcNow },
            (_, existing) =>
            {
                existing.TotalRequests++;
                existing.TotalElapsedMs += elapsedMs;
                if (isError) existing.Errors++;
                existing.LastAccessed = DateTime.UtcNow;
                return existing;
            });
    }

    private string CacheKey(string prefix, params object?[] args) =>
        $"{prefix}:{string.Join(":", args.Select(a => a?.ToString() ?? "null"))}";

    private async Task<T?> GetOrSetCachedAsync<T>(string key, Func<Task<T?>> factory)
    {
        if (_cache.TryGetValue(key, out var cached))
            return (T?)cached;

        var result = await factory();
        _cache.Set(key, result, TimeSpan.FromMinutes(5));
        return result;
    }

    #region Standard Reports

    /// <summary>
    /// Generate sales report for a specific date range
    /// </summary>
    [HttpGet("sales")]
    public async Task<ActionResult<SalesReport>> GetSalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? branchId = null,
        [FromQuery] Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        RequirePolicy(ReportingPolicies.ViewReports);
        var cacheKey = CacheKey("sales", startDate, endDate, branchId, warehouseId);
        var cached = await GetOrSetCachedAsync<SalesReport>(cacheKey, () => _reportingService.GenerateSalesReportAsync(startDate, endDate, branchId, warehouseId, cancellationToken));
        if (cached is not null)
        {
            _logger.LogInformation("Sales report served from cache: StartDate={StartDate}, EndDate={EndDate}, BranchId={BranchId}, WarehouseId={WarehouseId}",
                startDate, endDate, branchId, warehouseId);
            RecordMetric("GET /api/reporting/sales", 0, isError: false);
            return Ok(cached);
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Sales report requested: StartDate={StartDate}, EndDate={EndDate}, BranchId={BranchId}, WarehouseId={WarehouseId}",
            startDate, endDate, branchId, warehouseId);

        try
        {
            ValidateDateRange(startDate, endDate);

            var report = await _reportingService.GenerateSalesReportAsync(startDate, endDate, branchId, warehouseId, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Sales report generated successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);
            RecordMetric("GET /api/reporting/sales", sw.ElapsedMilliseconds, isError: false);
            return Ok(report);
        }
        catch (InvalidOperationException ex)
        {
            sw.Stop();
            _logger.LogWarning("Sales report validation failed: {Message} (ElapsedMs={ElapsedMs})", ex.Message, sw.ElapsedMilliseconds);
            RecordMetric("GET /api/reporting/sales", sw.ElapsedMilliseconds, isError: true);
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error generating sales report from {StartDate} to {EndDate} (ElapsedMs={ElapsedMs})", startDate, endDate, sw.ElapsedMilliseconds);
            RecordMetric("GET /api/reporting/sales", sw.ElapsedMilliseconds, isError: true);
            return StatusCode(500, new { error = "Failed to generate sales report", code = "server_error" });
        }
    }

    /// <summary>
    /// Generate inventory report for a specific location
    /// </summary>
    [HttpGet("inventory")]
    public async Task<ActionResult<InventoryReport>> GetInventoryReport(
        [FromQuery] Guid? branchId = null,
        [FromQuery] Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Inventory report requested: BranchId={BranchId}, WarehouseId={WarehouseId}", branchId, warehouseId);

        try
        {
            var report = await _reportingService.GenerateInventoryReportAsync(branchId, warehouseId, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Inventory report generated successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);
            return Ok(report);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error generating inventory report for branch {BranchId} and warehouse {WarehouseId} (ElapsedMs={ElapsedMs})", branchId, warehouseId, sw.ElapsedMilliseconds);
            return StatusCode(500, new { error = "Failed to generate inventory report", code = "server_error" });
        }
    }

    /// <summary>
    /// Generate customer report for a specific date range
    /// </summary>
    [HttpGet("customers")]
    public async Task<ActionResult<CustomerReport>> GetCustomerReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Customer report requested: StartDate={StartDate}, EndDate={EndDate}", startDate, endDate);

        try
        {
            ValidateDateRange(startDate, endDate);

            var report = await _reportingService.GenerateCustomerReportAsync(startDate, endDate, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Customer report generated successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);
            return Ok(report);
        }
        catch (InvalidOperationException ex)
        {
            sw.Stop();
            _logger.LogWarning("Customer report validation failed: {Message} (ElapsedMs={ElapsedMs})", ex.Message, sw.ElapsedMilliseconds);
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error generating customer report from {StartDate} to {EndDate} (ElapsedMs={ElapsedMs})", startDate, endDate, sw.ElapsedMilliseconds);
            return StatusCode(500, new { error = "Failed to generate customer report", code = "server_error" });
        }
    }

    /// <summary>
    /// Generate procurement report for a specific date range
    /// </summary>
    [HttpGet("procurement")]
    public async Task<ActionResult<ProcurementReport>> GetProcurementReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Procurement report requested: StartDate={StartDate}, EndDate={EndDate}", startDate, endDate);

        try
        {
            ValidateDateRange(startDate, endDate);

            var report = await _reportingService.GenerateProcurementReportAsync(startDate, endDate, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Procurement report generated successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);
            return Ok(report);
        }
        catch (InvalidOperationException ex)
        {
            sw.Stop();
            _logger.LogWarning("Procurement report validation failed: {Message} (ElapsedMs={ElapsedMs})", ex.Message, sw.ElapsedMilliseconds);
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error generating procurement report from {StartDate} to {EndDate} (ElapsedMs={ElapsedMs})", startDate, endDate, sw.ElapsedMilliseconds);
            return StatusCode(500, new { error = "Failed to generate procurement report", code = "server_error" });
        }
    }

    /// <summary>
    /// Generate financial report for a specific date range
    /// </summary>
    [HttpGet("financial")]
    public async Task<ActionResult<FinancialReport>> GetFinancialReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Financial report requested: StartDate={StartDate}, EndDate={EndDate}", startDate, endDate);

        try
        {
            ValidateDateRange(startDate, endDate);

            var report = await _reportingService.GenerateFinancialReportAsync(startDate, endDate, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Financial report generated successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);
            return Ok(report);
        }
        catch (InvalidOperationException ex)
        {
            sw.Stop();
            _logger.LogWarning("Financial report validation failed: {Message} (ElapsedMs={ElapsedMs})", ex.Message, sw.ElapsedMilliseconds);
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error generating financial report from {StartDate} to {EndDate} (ElapsedMs={ElapsedMs})", startDate, endDate, sw.ElapsedMilliseconds);
            return StatusCode(500, new { error = "Failed to generate financial report", code = "server_error" });
        }
    }

    #endregion

    #region Advanced Analytics

    /// <summary>
    /// Get comprehensive sales analytics with growth metrics and trends
    /// </summary>
    [HttpGet("analytics/sales")]
    public async Task<ActionResult<SalesAnalytics>> GetSalesAnalytics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
        [FromQuery] Guid? branchId = null,
        [FromQuery] Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        RequirePolicy(ReportingPolicies.ViewAnalytics);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Sales analytics requested: StartDate={StartDate}, EndDate={EndDate}, Period={Period}, BranchId={BranchId}, WarehouseId={WarehouseId}",
            startDate, endDate, period, branchId, warehouseId);

        try
        {
            ValidateDateRange(startDate, endDate);

            var analytics = await _analyticsService.GetSalesAnalyticsAsync(startDate, endDate,  period, branchId, warehouseId, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Sales analytics generated successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);
            return Ok(analytics);
        }
        catch (InvalidOperationException ex)
        {
            sw.Stop();
            _logger.LogWarning("Sales analytics validation failed: {Message} (ElapsedMs={ElapsedMs})", ex.Message, sw.ElapsedMilliseconds);
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error generating sales analytics from {StartDate} to {EndDate} (ElapsedMs={ElapsedMs})", startDate, endDate, sw.ElapsedMilliseconds);
            return StatusCode(500, new { error = "Failed to generate sales analytics", code = "server_error" });
        }
    }

    /// <summary>
    /// Get inventory analytics with turnover, health, and demand forecasting
    /// </summary>
    [HttpGet("analytics/inventory")]
    public async Task<ActionResult<InventoryAnalytics>> GetInventoryAnalytics(
        [FromQuery] Guid? branchId = null,
        [FromQuery] Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var analytics = await _analyticsService.GetInventoryAnalyticsAsync(branchId, warehouseId, cancellationToken);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating inventory analytics for branch {BranchId} and warehouse {WarehouseId}", branchId, warehouseId);
            return StatusCode(500, new { error = "Failed to generate inventory analytics", code = "server_error" });
        }
    }

    /// <summary>
    /// Get customer analytics with acquisition, retention, and lifetime value metrics
    /// </summary>
    [HttpGet("analytics/customers")]
    public async Task<ActionResult<CustomerAnalytics>> GetCustomerAnalytics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateDateRange(startDate, endDate);

            var analytics = await _analyticsService.GetCustomerAnalyticsAsync(startDate, endDate, cancellationToken);
            return Ok(analytics);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating customer analytics from {StartDate} to {EndDate}", startDate, endDate);
            return StatusCode(500, new { error = "Failed to generate customer analytics", code = "server_error" });
        }
    }

    /// <summary>
    /// Get financial analytics with profitability, cash flow, and ratio analysis
    /// </summary>
    [HttpGet("analytics/financial")]
    public async Task<ActionResult<FinancialAnalytics>> GetFinancialAnalytics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateDateRange(startDate, endDate);

            var analytics = await _analyticsService.GetFinancialAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial analytics from {StartDate} to {EndDate}", startDate, endDate);
            return StatusCode(500, new { error = "Failed to generate financial analytics", code = "server_error" });
        }
    }

    /// <summary>
    /// Get predictive analytics for forecasting and trend analysis
    /// </summary>
    [HttpGet("analytics/predictive")]
    public async Task<ActionResult<PredictiveAnalytics>> GetPredictiveAnalytics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] PredictionType model = PredictionType.Sales,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateDateRange(startDate, endDate);

            var analytics = await _analyticsService.GetPredictiveAnalyticsAsync(startDate, endDate, model, cancellationToken);
            return Ok(analytics);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating predictive analytics from {StartDate} to {EndDate}", startDate, endDate);
            return StatusCode(500, new { error = "Failed to generate predictive analytics", code = "server_error" });
        }
    }

    #endregion

    #region Custom Reports

    [HttpPost("custom/run")]
    public async Task<ActionResult<object>> RunCustomReport(
        [FromBody] CustomReportRunRequest request,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Custom report run requested: Name={Name}, Dataset={Dataset}, StartDate={StartDate}, EndDate={EndDate}, BranchId={BranchId}, WarehouseId={WarehouseId}",
            request.Name, request.Dataset, request.StartDate, request.EndDate, request.BranchId, request.WarehouseId);

        try
        {
            if (!ModelState.IsValid)
            {
                sw.Stop();
                _logger.LogWarning("Custom report run validation failed: {Errors} (ElapsedMs={ElapsedMs})", string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), sw.ElapsedMilliseconds);
                return BadRequest(ModelState);
            }

            object report = request.Dataset.ToLower() switch
            {
                "sales" => await _reportingService.GenerateSalesReportAsync(request.StartDate, request.EndDate, request.BranchId, request.WarehouseId, cancellationToken),
                "inventory" => await _reportingService.GenerateInventoryReportAsync(request.BranchId, request.WarehouseId, cancellationToken),
                "customers" => await _reportingService.GenerateCustomerReportAsync(request.StartDate, request.EndDate, cancellationToken),
                "procurement" => await _reportingService.GenerateProcurementReportAsync(request.StartDate, request.EndDate, cancellationToken),
                "financial" => await _reportingService.GenerateFinancialReportAsync(request.StartDate, request.EndDate, cancellationToken),
                _ => throw new ArgumentException($"Unsupported dataset: {request.Dataset}")
            };

            sw.Stop();
            _logger.LogInformation("Custom report run completed successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);

            var result = new
            {
                name = request.Name,
                dataset = request.Dataset,
                startDate = request.StartDate,
                endDate = request.EndDate,
                generatedAt = DateTime.UtcNow,
                data = report
            };

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            sw.Stop();
            _logger.LogWarning("Custom report run argument error: {Message} (ElapsedMs={ElapsedMs})", ex.Message, sw.ElapsedMilliseconds);
            return BadRequest(new { error = ex.Message, code = "validation_error" });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error running custom report {Name} for dataset {Dataset} (ElapsedMs={ElapsedMs})", request.Name, request.Dataset, sw.ElapsedMilliseconds);
            return StatusCode(500, new { error = "Failed to run custom report", code = "server_error" });
        }
    }

    [HttpPost("custom/export")]
    public async Task<ActionResult<ExportResult>> ExportCustomReport(
        [FromBody] CustomReportExportRequest request,
        CancellationToken cancellationToken = default)
    {
        RequirePolicy(ReportingPolicies.ExportReports);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Custom report export requested: Name={Name}, Dataset={Dataset}, Format={Format}", request.Name, request.Dataset, request.Format);

        try
        {
            if (!ModelState.IsValid)
            {
                sw.Stop();
                _logger.LogWarning("Custom report export validation failed: {Errors} (ElapsedMs={ElapsedMs})", string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), sw.ElapsedMilliseconds);
                return BadRequest(ModelState);
            }

            await Task.CompletedTask;

            var exportResult = new ExportResult
            {
                FileId = Guid.NewGuid(),
                FileName = $"custom_report_{DateTime.UtcNow:yyyyMMddHHmmss}.{request.Format}",
                Format = request.Format,
                Status = "Processing",
                EstimatedCompletion = DateTime.UtcNow.AddMinutes(5)
            };

            sw.Stop();
            _logger.LogInformation("Custom report export queued successfully in {ElapsedMs}ms: FileId={FileId}", sw.ElapsedMilliseconds, exportResult.FileId);
            return Ok(exportResult);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error exporting custom report {Name} in {Format} format (ElapsedMs={ElapsedMs})", request.Name, request.Format, sw.ElapsedMilliseconds);
            return StatusCode(500, new { error = "Failed to export custom report", code = "server_error" });
        }
    }

    #endregion

    #region Dashboard Data

    /// <summary>
    /// Get dashboard summary with key metrics
    /// </summary>
    [HttpGet("dashboard/summary")]
    public async Task<ActionResult<DashboardSummary>> GetDashboardSummary(
        [FromQuery] Guid? locationId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var summary = await _reportingService.GetDashboardSummaryAsync(locationId, cancellationToken);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating dashboard summary for location {LocationId}", locationId);
            return StatusCode(500, new { error = "Failed to generate dashboard summary", code = "server_error" });
        }
    }

    /// <summary>
    /// Get real-time metrics for dashboard
    /// </summary>
    [HttpGet("dashboard/realtime")]
    public async Task<ActionResult<RealTimeMetrics>> GetRealTimeMetrics(
        [FromQuery] Guid? locationId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var metrics = await _reportingService.GetRealTimeMetricsAsync(locationId, cancellationToken);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching real-time metrics for location {LocationId}", locationId);
            return StatusCode(500, new { error = "Failed to fetch real-time metrics", code = "server_error" });
        }
    }

    #endregion

    #region Export and Scheduling

    /// <summary>
    /// Export any report in specified format
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportReport(
        [FromBody] ExportRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.CompletedTask;

            var exportResult = new ExportResult
            {
                FileId = Guid.NewGuid(),
                FileName = $"report_{DateTime.UtcNow:yyyyMMddHHmmss}.{request.Format}",
                Format = request.Format,
                Status = "Processing",
                EstimatedCompletion = DateTime.UtcNow.AddMinutes(5)
            };

            return Ok(exportResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report in {Format} format", request.Format);
            return StatusCode(500, new { error = "Failed to export report", code = "server_error" });
        }
    }

    /// <summary>
    /// Get export status
    /// </summary>
    [HttpGet("export/{fileId}/status")]
    public async Task<ActionResult<ExportResult>> GetExportStatus(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = new ExportResult
            {
                FileId = fileId,
                FileName = $"report_{fileId:N}.pdf",
                Format = "pdf",
                Status = "Completed",
                DownloadUrl = $"/api/reporting/export/{fileId}/download",
                GeneratedAt = DateTime.UtcNow
            };

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching export status for file {FileId}", fileId);
            return StatusCode(500, new { error = "Failed to fetch export status", code = "server_error" });
        }
    }

    /// <summary>
    /// Download exported report
    /// </summary>
    [HttpGet("export/{fileId}/download")]
    public async Task<IActionResult> DownloadExport(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fileBytes = Array.Empty<byte>();
            return File(fileBytes, "application/pdf", $"report_{fileId:N}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading exported file {FileId}", fileId);
            return StatusCode(500, new { error = "Failed to download exported file", code = "server_error" });
        }
    }

    #endregion

    #region Performance Monitoring

    /// <summary>
    /// Get performance metrics for reporting endpoints
    /// </summary>
    [HttpGet("metrics")]
    public ActionResult<IEnumerable<ReportingMetrics>> GetMetrics()
    {
        RequirePolicy(ReportingPolicies.ManageReports);
        return Ok(_metrics.Values.OrderByDescending(m => m.LastAccessed));
    }

    /// <summary>
    /// Get system health and alerting status
    /// </summary>
    [HttpGet("alerts")]
    public ActionResult<object> GetAlerts()
    {
        RequirePolicy(ReportingPolicies.ManageReports);
        var alerts = new List<object>();
        var avgMs = _metrics.Values.Any() ? _metrics.Values.Average(m => m.AverageMs) : 0;
        var errorRate = _metrics.Values.Any() ? (double)_metrics.Values.Sum(m => m.Errors) / _metrics.Values.Sum(m => m.TotalRequests) * 100 : 0;

        if (errorRate > 5)
            alerts.Add(new { type = "error_rate", message = $"High error rate: {errorRate:F1}%", threshold = "5%" });

        if (avgMs > 2000)
            alerts.Add(new { type = "slow_response", message = $"Average response time: {avgMs:F0}ms", threshold = "2000ms" });

        if (_metrics.Values.Any(m => m.Errors > 10))
            alerts.Add(new { type = "error_spike", message = "Multiple errors detected", count = _metrics.Values.Sum(m => m.Errors) });

        return Ok(new { status = "active", alerts, timestamp = DateTime.UtcNow });
    }

    #endregion

    #region Supporting DTOs

    public class DashboardSummary
    {
        public decimal TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int LowStockItems { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class RealTimeMetrics
    {
        public decimal CurrentRevenue { get; set; }
        public int ActiveOrders { get; set; }
        public int OnlineCustomers { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ExportRequest
    {
        [Required]
        public string ReportType { get; set; } = string.Empty;

        [Required]
        public string Format { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? WarehouseId { get; set; }
    }

    public class ExportResult
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? DownloadUrl { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
    }

    public class CustomReportRunRequest : IValidatableObject
    {
        [Required]
        [Sanitized(allowHtml: false, maxLength: 100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Dataset { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public Guid? BranchId { get; set; }
        public Guid? WarehouseId { get; set; }
        public List<CustomReportFilter> Filters { get; set; } = new();
        public List<string> GroupBy { get; set; } = new();
        public List<string> Metrics { get; set; } = new();
        [Sanitized(allowHtml: false, maxLength: 20)]
        public string Format { get; set; } = "json";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
                yield return new ValidationResult("StartDate must be less than or equal to EndDate", new[] { nameof(StartDate) });

            var validDatasets = new[] { "sales", "inventory", "customers", "procurement", "financial" };
            if (!validDatasets.Contains(Dataset?.ToLower() ?? string.Empty))
                yield return new ValidationResult($"Dataset must be one of: {string.Join(", ", validDatasets)}", new[] { nameof(Dataset) });

            yield break;
        }
    }

    public class CustomReportExportRequest
    {
        [Required]
        [Sanitized(allowHtml: false, maxLength: 100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Dataset { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public Guid? BranchId { get; set; }
        public Guid? WarehouseId { get; set; }
        public List<CustomReportFilter> Filters { get; set; } = new();
        public List<string> GroupBy { get; set; } = new();
        public List<string> Metrics { get; set; } = new();
        [Required]
        [Sanitized(allowHtml: false, maxLength: 10)]
        public string Format { get; set; } = string.Empty;
    }

    public class CustomReportFilter
    {
        public string Field { get; set; } = string.Empty;
        public string Op { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    #endregion

    #region Minimal DTOs for report types to resolve compilation

    public class LowStockAlert
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
    }

    public class InventoryMovement
    {
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
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
        public DateTime GeneratedAt { get; set; }
    }

    public class CategoryStockData
    {
        public string Category { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
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

    public class MonthlyFinancialData
    {
        public DateTime Month { get; set; }
        public decimal Revenue { get; set; }
        public decimal Costs { get; set; }
        public decimal Profit { get; set; }
    }

    #endregion
}
