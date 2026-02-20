using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace NationalClothingStore.Infrastructure.Monitoring;

/// <summary>
/// Database performance monitoring service
/// </summary>
public class DatabasePerformanceMonitor : IHostedService
{
    private readonly ILogger<DatabasePerformanceMonitor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Timer _monitoringTimer;

    public DatabasePerformanceMonitor(
        ILogger<DatabasePerformanceMonitor> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        
        // Monitor every 5 minutes
        _monitoringTimer = new Timer(MonitorPerformance, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Database performance monitoring started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _monitoringTimer?.Dispose();
        _logger.LogInformation("Database performance monitoring stopped");
        return Task.CompletedTask;
    }

    private async void MonitorPerformance(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var healthCheckService = scope.ServiceProvider.GetRequiredService<HealthCheckService>();
            
            var healthCheckResult = await healthCheckService.CheckHealthAsync();
            
            var databaseCheck = healthCheckResult.Entries
                .FirstOrDefault(e => e.Key == "database");
            
            if (databaseCheck.Key != null)
            {
                LogPerformanceMetrics(databaseCheck.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database performance monitoring");
        }
    }

    private void LogPerformanceMetrics(HealthReportEntry databaseCheck)
    {
        var status = databaseCheck.Status.ToString();
        var duration = databaseCheck.Duration.TotalMilliseconds;
        
        _logger.LogInformation(
            "Database Health Check - Status: {Status}, Duration: {Duration}ms, Data: {@Data}",
            status,
            duration,
            databaseCheck.Data);
        
        // Log warnings for degraded performance
        if (databaseCheck.Status == HealthStatus.Degraded)
        {
            _logger.LogWarning("Database performance is degraded");
        }
        
        if (databaseCheck.Status == HealthStatus.Unhealthy)
        {
            _logger.LogError("Database is unhealthy!");
        }
    }
}

/// <summary>
/// Database performance metrics
/// </summary>
public class DatabasePerformanceMetrics
{
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = string.Empty;
    public double DurationMs { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public string ConnectionString { get; set; } = string.Empty;
    public int ActiveConnections { get; set; }
    public int IdleConnections { get; set; }
    public double AverageResponseTime { get; set; }
    public int QueriesPerSecond { get; set; }
    public long TotalQueries { get; set; }
    public long FailedQueries { get; set; }
}
