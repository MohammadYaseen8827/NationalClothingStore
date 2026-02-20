using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NationalClothingStore.Infrastructure.HealthChecks;
using Microsoft.Extensions.Logging;

namespace NationalClothingStore.Infrastructure.HealthChecks;

/// <summary>
/// Extension methods for configuring health checks
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds health checks to the service collection
    /// </summary>
    public static IHealthChecksBuilder AddHealthChecks(this IServiceCollection services)
    {
        return Microsoft.Extensions.DependencyInjection.HealthCheckServiceCollectionExtensions.AddHealthChecks(services)
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck<RedisHealthCheck>("redis")
            .AddCheck<MemoryHealthCheck>("memory");
    }
}

/// <summary>
/// Memory health check for monitoring application memory usage
/// </summary>
public class MemoryHealthCheck(ILogger<MemoryHealthCheck> logger) : IHealthCheck
{
    private readonly ILogger<MemoryHealthCheck> _logger = logger;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var memoryUsage = GC.GetTotalMemory(false);
            var availableMemory = GC.GetTotalMemory(true);
            
            var memoryUsagePercentage = (double)memoryUsage / availableMemory * 100;
            
            var status = memoryUsagePercentage switch
            {
                > 90 => HealthStatus.Unhealthy,
                > 75 => HealthStatus.Degraded,
                _ => HealthStatus.Healthy
            };

            var data = new Dictionary<string, object>
            {
                ["used_memory_mb"] = memoryUsage / 1024 / 1024.0,
                ["available_memory_mb"] = availableMemory / 1024 / 1024.0,
                ["usage_percentage"] = Math.Round(memoryUsagePercentage, 2)
            };

            var message = status switch
            {
                HealthStatus.Healthy => "Memory usage is normal",
                HealthStatus.Degraded => "Memory usage is high",
                HealthStatus.Unhealthy => "Memory usage is critical",
                _ => "Memory status unknown"
            };

            return Task.FromResult(new HealthCheckResult(status, message, null, data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Memory health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("Memory check failed", ex));
        }
    }
}
