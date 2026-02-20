using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace NationalClothingStore.Infrastructure.HealthChecks;

/// <summary>
/// Redis connectivity health check
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisHealthCheck> _logger;

    public RedisHealthCheck(IConnectionMultiplexer redis, ILogger<RedisHealthCheck> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Test Redis connectivity by executing a simple command
            var database = _redis.GetDatabase(0);
            await database.StringSetAsync("health_check", "healthy", TimeSpan.FromSeconds(5));
            await database.KeyDeleteAsync("health_check");
            
            var data = new Dictionary<string, object>
            {
                ["server"] = "Redis",
                ["status"] = "connected",
                ["endpoints"] = _redis.GetEndPoints().Count()
            };

            return HealthCheckResult.Healthy("Redis connection is healthy", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis health check failed");
            
            return HealthCheckResult.Unhealthy("Redis connection failed", ex);
        }
    }
}
