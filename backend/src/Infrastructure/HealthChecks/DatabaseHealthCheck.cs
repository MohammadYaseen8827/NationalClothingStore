using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Infrastructure.Data;
using System.Data;

namespace NationalClothingStore.Infrastructure.HealthChecks;

/// <summary>
/// Database connectivity health check
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly NationalClothingStoreDbContext _dbContext;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(NationalClothingStoreDbContext dbContext, ILogger<DatabaseHealthCheck> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Test database connectivity
            var connection = _dbContext.Database.GetDbConnection();
            
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
            }

            // Test database query
            await _dbContext.Database.CanConnectAsync(cancellationToken);

            // Get database info
            var databaseName = _dbContext.Database.GetDbConnection().Database;
            
            var data = new Dictionary<string, object>
            {
                ["database"] = databaseName,
                ["connection_state"] = connection.State.ToString(),
                ["server_version"] = connection.ServerVersion
            };

            return HealthCheckResult.Healthy("Database connection is healthy", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}
