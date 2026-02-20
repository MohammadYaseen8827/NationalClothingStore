using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace NationalClothingStore.Infrastructure.Monitoring;

/// <summary>
/// Performance monitoring for sales operations
/// </summary>
public class SalesPerformanceMonitor : IHostedService
{
    private readonly ILogger<SalesPerformanceMonitor> _logger;
    private readonly Timer _reportingTimer;
    private readonly ConcurrentDictionary<string, OperationMetrics> _operationMetrics;
    private readonly object _lock = new();

    public SalesPerformanceMonitor(ILogger<SalesPerformanceMonitor> logger)
    {
        _logger = logger;
        _operationMetrics = new ConcurrentDictionary<string, OperationMetrics>();
        _reportingTimer = new Timer(LogPeriodicMetrics, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sales performance monitoring started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _reportingTimer?.Dispose();
        LogFinalMetrics();
        _logger.LogInformation("Sales performance monitoring stopped");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Record a sales operation
    /// </summary>
    public void RecordOperation(string operationName, TimeSpan duration, bool success = true)
    {
        var metrics = _operationMetrics.GetOrAdd(operationName, _ => new OperationMetrics());
        
        lock (_lock)
        {
            metrics.TotalOperations++;
            metrics.TotalDuration += duration;
            if (success)
            {
                metrics.SuccessfulOperations++;
            }
            else
            {
                metrics.FailedOperations++;
            }

            if (duration < metrics.MinDuration || metrics.MinDuration == TimeSpan.Zero)
                metrics.MinDuration = duration;
            
            if (duration > metrics.MaxDuration)
                metrics.MaxDuration = duration;
        }
    }

    /// <summary>
    /// Record inventory update performance
    /// </summary>
    public void RecordInventoryUpdate(int itemsUpdated, TimeSpan duration)
    {
        var metrics = _operationMetrics.GetOrAdd("InventoryUpdate", _ => new OperationMetrics());
        
        lock (_lock)
        {
            metrics.TotalOperations++;
            metrics.TotalDuration += duration;
            metrics.SuccessfulOperations++;
            
            // Track items per operation
            if (!metrics.CustomMetrics.ContainsKey("ItemsUpdated"))
                metrics.CustomMetrics["ItemsUpdated"] = 0;
            
            metrics.CustomMetrics["ItemsUpdated"] = (int)metrics.CustomMetrics["ItemsUpdated"] + itemsUpdated;
        }
    }

    /// <summary>
    /// Record payment processing performance
    /// </summary>
    public void RecordPaymentProcessing(string paymentMethod, TimeSpan duration, bool success = true)
    {
        var operationName = $"Payment_{paymentMethod}";
        RecordOperation(operationName, duration, success);
    }

    private void LogPeriodicMetrics(object? state)
    {
        try
        {
            var snapshot = new Dictionary<string, OperationMetrics>();
            foreach (var kvp in _operationMetrics)
            {
                snapshot[kvp.Key] = kvp.Value.Clone();
            }

            foreach (var kvp in snapshot)
            {
                LogOperationMetrics(kvp.Key, kvp.Value);
            }

            // Clear metrics for next period
            _operationMetrics.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during periodic sales metrics logging");
        }
    }

    private void LogOperationMetrics(string operationName, OperationMetrics metrics)
    {
        if (metrics.TotalOperations == 0) return;

        var avgDuration = metrics.TotalDuration.TotalMilliseconds / metrics.TotalOperations;
        var successRate = (double)metrics.SuccessfulOperations / metrics.TotalOperations * 100;

        _logger.LogInformation(
            "Sales Operation Metrics - Operation: {Operation}, Count: {Count}, AvgDuration: {AvgDuration}ms, " +
            "Min: {Min}ms, Max: {Max}ms, SuccessRate: {SuccessRate:F2}%, Failures: {Failures}",
            operationName,
            metrics.TotalOperations,
            avgDuration,
            metrics.MinDuration.TotalMilliseconds,
            metrics.MaxDuration.TotalMilliseconds,
            successRate,
            metrics.FailedOperations);

        // Log warnings for poor performance
        if (avgDuration > 5000) // 5 seconds
        {
            _logger.LogWarning("Slow sales operation detected: {Operation} took {Duration}ms on average", 
                operationName, avgDuration);
        }

        if (successRate < 95)
        {
            _logger.LogWarning("Low success rate for sales operation: {Operation} has {SuccessRate:F2}% success rate",
                operationName, successRate);
        }

        // Log custom metrics
        foreach (var customMetric in metrics.CustomMetrics)
        {
            _logger.LogDebug("Custom Metric - Operation: {Operation}, Metric: {MetricName}, Value: {Value}",
                operationName, customMetric.Key, customMetric.Value);
        }
    }

    private void LogFinalMetrics()
    {
        _logger.LogInformation("Final sales performance metrics:");
        foreach (var kvp in _operationMetrics)
        {
            LogOperationMetrics(kvp.Key, kvp.Value);
        }
    }
}

/// <summary>
/// Metrics for a specific operation
/// </summary>
public class OperationMetrics
{
    public int TotalOperations { get; set; }
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public TimeSpan MinDuration { get; set; }
    public TimeSpan MaxDuration { get; set; }
    public Dictionary<string, object> CustomMetrics { get; set; } = new();

    public OperationMetrics Clone()
    {
        return new OperationMetrics
        {
            TotalOperations = TotalOperations,
            SuccessfulOperations = SuccessfulOperations,
            FailedOperations = FailedOperations,
            TotalDuration = TotalDuration,
            MinDuration = MinDuration,
            MaxDuration = MaxDuration,
            CustomMetrics = new Dictionary<string, object>(CustomMetrics)
        };
    }
}

/// <summary>
/// Helper class for measuring operation performance
/// </summary>
public class OperationTimer : IDisposable
{
    private readonly SalesPerformanceMonitor _monitor;
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch;
    private bool _success = true;

    public OperationTimer(SalesPerformanceMonitor monitor, string operationName)
    {
        _monitor = monitor;
        _operationName = operationName;
        _stopwatch = Stopwatch.StartNew();
    }

    public void SetFailure()
    {
        _success = false;
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _monitor.RecordOperation(_operationName, _stopwatch.Elapsed, _success);
    }
}