using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.Infrastructure.Jobs;

/// <summary>
/// Background job for generating scheduled reports
/// </summary>
public class ReportGenerationJob : IBackgroundJob
{
    private readonly IReportingService _reportingService;
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<ReportGenerationJob> _logger;
    private readonly IConfiguration _configuration;

    public ReportGenerationJob(
        IReportingService reportingService,
        IAnalyticsService analyticsService,
        ILogger<ReportGenerationJob> logger,
        IConfiguration configuration)
    {
        _reportingService = reportingService;
        _analyticsService = analyticsService;
        _logger = logger;
        _configuration = configuration;
    }

    public string Name => "Report Generation Job";

    public async Task ExecuteAsync(JobExecutionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting scheduled report generation job at {Time}", DateTime.UtcNow);

            // Get report configuration from job data
            var reportConfig = GetReportConfiguration(context.JobData);
            
            if (reportConfig == null)
            {
                _logger.LogWarning("No report configuration found for job {JobId}", context.JobId);
                return;
            }

            // Generate reports based on configuration
            await GenerateReportsAsync(reportConfig, cancellationToken);

            _logger.LogInformation("Completed scheduled report generation job for {ReportCount} reports", reportConfig.Reports.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing report generation job");
            throw;
        }
    }

    private ReportConfiguration? GetReportConfiguration(Dictionary<string, object> jobData)
    {
        try
        {
            if (!jobData.TryGetValue("ReportConfig", out var configObj))
            {
                return null;
            }

            // In a real implementation, this would deserialize the configuration
            // For now, we'll return a default configuration
            return new ReportConfiguration
            {
                Reports =
                [
                    new()
                    {
                        ReportType = "Sales", Schedule = "Daily",
                        Recipients = ["manager@company.com"]
                    },
                    new()
                    {
                        ReportType = "Inventory", Schedule = "Daily",
                        Recipients = ["inventory@company.com"]
                    },
                    new()
                    {
                        ReportType = "Financial", Schedule = "Weekly",
                        Recipients = ["finance@company.com"]
                    },
                    new()
                    {
                        ReportType = "Customer", Schedule = "Weekly",
                        Recipients = ["marketing@company.com"]
                    }
                ]
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing report configuration");
            return null;
        }
    }

    private async Task GenerateReportsAsync(ReportConfiguration config, CancellationToken cancellationToken)
    {
        var endDate = DateTime.UtcNow;

        foreach (var reportSchedule in config.Reports)
        {
            var startDate = CalculateStartDate(reportSchedule.Schedule, endDate);
            
            try
            {
                await GenerateSingleReportAsync(reportSchedule, startDate, endDate, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating {ReportType} report", reportSchedule.ReportType);
                // Continue with other reports even if one fails
            }
        }
    }

    private async Task GenerateSingleReportAsync(ReportSchedule schedule, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating {ReportType} report from {StartDate} to {EndDate}", schedule.ReportType, startDate, endDate);

        switch (schedule.ReportType.ToLower())
        {
            case "sales":
                await GenerateSalesReportAsync(schedule, startDate, endDate, cancellationToken);
                break;
            
            case "inventory":
                await GenerateInventoryReportAsync(schedule, startDate, endDate, cancellationToken);
                break;
            
            case "customer":
                await GenerateCustomerReportAsync(schedule, startDate, endDate, cancellationToken);
                break;
            
            case "financial":
                await GenerateFinancialReportAsync(schedule, startDate, endDate, cancellationToken);
                break;
            
            case "procurement":
                await GenerateProcurementReportAsync(schedule, startDate, endDate, cancellationToken);
                break;
            
            case "analytics":
                await GenerateAnalyticsReportAsync(schedule, startDate, endDate, cancellationToken);
                break;
            
            default:
                _logger.LogWarning("Unknown report type: {ReportType}", schedule.ReportType);
                break;
        }
    }

    private async Task GenerateSalesReportAsync(ReportSchedule schedule, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var report = await _reportingService.GenerateSalesReportAsync(startDate, endDate, null, null, cancellationToken);
        
        // Save report to storage and send notifications
        await SaveAndNotifyReportAsync(schedule, report, "Sales Report", cancellationToken);
    }

    private async Task GenerateInventoryReportAsync(ReportSchedule schedule, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var report = await _reportingService.GenerateInventoryReportAsync(null, null, cancellationToken);
        
        await SaveAndNotifyReportAsync(schedule, report, "Inventory Report", cancellationToken);
    }

    private async Task GenerateCustomerReportAsync(ReportSchedule schedule, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var report = await _reportingService.GenerateCustomerReportAsync(startDate, endDate, cancellationToken);
        
        await SaveAndNotifyReportAsync(schedule, report, "Customer Report", cancellationToken);
    }

    private async Task GenerateFinancialReportAsync(ReportSchedule schedule, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var report = await _reportingService.GenerateFinancialReportAsync(startDate, endDate, cancellationToken);
        
        await SaveAndNotifyReportAsync(schedule, report, "Financial Report", cancellationToken);
    }

    private async Task GenerateProcurementReportAsync(ReportSchedule schedule, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var report = await _reportingService.GenerateProcurementReportAsync(startDate, endDate, cancellationToken);
        
        await SaveAndNotifyReportAsync(schedule, report, "Procurement Report", cancellationToken);
    }

    private async Task GenerateAnalyticsReportAsync(ReportSchedule schedule, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var salesAnalytics = await _analyticsService.GetSalesAnalyticsAsync(startDate, endDate, AnalyticsPeriod.Daily, null, null, cancellationToken);
        var inventoryAnalytics = await _analyticsService.GetInventoryAnalyticsAsync(null, null, cancellationToken);
        var customerAnalytics = await _analyticsService.GetCustomerAnalyticsAsync(startDate, endDate, cancellationToken);
        var financialAnalytics = await _analyticsService.GetFinancialAnalyticsAsync(startDate, endDate, AnalyticsPeriod.Monthly, cancellationToken);

        var analyticsReport = new AnalyticsReport
        {
            SalesAnalytics = salesAnalytics,
            InventoryAnalytics = inventoryAnalytics,
            CustomerAnalytics = customerAnalytics,
            FinancialAnalytics = financialAnalytics,
            GeneratedAt = DateTime.UtcNow,
            Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}"
        };

        await SaveAndNotifyReportAsync(schedule, analyticsReport, "Analytics Report", cancellationToken);
    }

    private async Task SaveAndNotifyReportAsync(ReportSchedule schedule, object report, string reportName, CancellationToken cancellationToken)
    {
        try
        {
            // Save report to storage (file system, cloud storage, etc.)
            var filePath = await SaveReportToStorageAsync(report, reportName, cancellationToken);
            
            // Send notifications to recipients
            await SendReportNotificationsAsync(schedule, filePath, reportName, cancellationToken);
            
            _logger.LogInformation("Successfully generated and distributed {ReportName}", reportName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving or notifying {ReportName}", reportName);
            throw;
        }
    }

    private async Task<string> SaveReportToStorageAsync(object report, string reportName, CancellationToken cancellationToken)
    {
        try
        {
            // Generate file path
            var fileName = $"{reportName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var storagePath = _configuration["ReportStorage:Path"] ?? "/reports";
            var fullPath = Path.Combine(storagePath, fileName);

            // Ensure directory exists
            Directory.CreateDirectory(storagePath);

            // Serialize report to JSON
            var json = System.Text.Json.JsonSerializer.Serialize(report, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            // Write to file
            await File.WriteAllTextAsync(fullPath, json, cancellationToken);

            _logger.LogInformation("Report saved to {FilePath}", fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving report to storage");
            throw;
        }
    }

    private async Task SendReportNotificationsAsync(ReportSchedule schedule, string filePath, string reportName, CancellationToken cancellationToken)
    {
        try
        {
            // In a real implementation, this would send emails or other notifications
            // For now, we'll just log the notification
            
            var recipients = string.Join(", ", schedule.Recipients);
            _logger.LogInformation("Report notification sent for {ReportName} to {Recipients}", reportName, recipients);

            // Example email notification (placeholder)
            foreach (var recipient in schedule.Recipients)
            {
                await SendEmailNotificationAsync(recipient, reportName, filePath, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending report notifications");
            throw;
        }
    }

    private async Task SendEmailNotificationAsync(string recipient, string reportName, string filePath, CancellationToken cancellationToken)
    {
        // Placeholder for email sending implementation
        // In a real implementation, this would use an email service like SendGrid, SMTP, etc.
        
        _logger.LogInformation("Email notification sent to {Recipient} for {ReportName}", recipient, reportName);
        
        // Simulate async operation
        await Task.Delay(100, cancellationToken);
    }

    private DateTime CalculateStartDate(string schedule, DateTime endDate)
    {
        return schedule.ToLower() switch
        {
            "daily" => endDate.AddDays(-1),
            "weekly" => endDate.AddDays(-7),
            "monthly" => endDate.AddMonths(-1),
            "quarterly" => endDate.AddMonths(-3),
            "yearly" => endDate.AddYears(-1),
            _ => endDate.AddDays(-1) // Default to daily
        };
    }
}

// Supporting data classes
public class ReportConfiguration
{
    public List<ReportSchedule> Reports { get; set; } = new();
}

public class ReportSchedule
{
    public string ReportType { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty; // Daily, Weekly, Monthly, etc.
    public List<string> Recipients { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string Format { get; set; } = "JSON"; // JSON, PDF, Excel, CSV
    public bool IsEnabled { get; set; } = true;
}

public class AnalyticsReport
{
    public SalesAnalytics SalesAnalytics { get; set; } = new();
    public InventoryAnalytics InventoryAnalytics { get; set; } = new();
    public CustomerAnalytics CustomerAnalytics { get; set; } = new();
    public FinancialAnalytics FinancialAnalytics { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public string Period { get; set; } = string.Empty;
}
