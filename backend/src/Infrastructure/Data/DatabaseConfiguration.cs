using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NationalClothingStore.Infrastructure.HealthChecks;

namespace NationalClothingStore.Infrastructure.Data;

/// <summary>
/// Database configuration extensions
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Configures the database context with optimal performance settings
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<NationalClothingStoreDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Connection pooling settings
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                npgsqlOptions.MaxBatchSize(100);
            });

            // Performance settings
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            options.EnableServiceProviderCaching();

            // Query performance
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableServiceProviderCaching();
        });

        // Configure connection pooling
        services.Configure<NpgsqlConnectionStringBuilder>(connectionString, builder =>
        {
            builder.Pooling = true;
            builder.MinPoolSize = 5;
            builder.MaxPoolSize = 100;
            builder.ConnectionIdleLifetime = 300; // 5 minutes
            builder.ConnectionPruningInterval = 10; // 10 seconds
            builder.Timeout = 30; // 30 seconds
            builder.CommandTimeout = 30; // 30 seconds
        });

        return services;
    }

    /// <summary>
    /// Configures database performance monitoring
    /// </summary>
    public static IServiceCollection AddDatabasePerformanceMonitoring(this IServiceCollection services)
    {
        Microsoft.Extensions.DependencyInjection.HealthCheckServiceCollectionExtensions.AddHealthChecks(services)
            .AddCheck<DatabaseHealthCheck>("database");

        return services;
    }
}

/// <summary>
/// Database performance settings
/// </summary>
public class DatabasePerformanceSettings
{
    public int MaxPoolSize { get; set; } = 100;
    public int MinPoolSize { get; set; } = 5;
    public int ConnectionTimeout { get; set; } = 30;
    public int CommandTimeout { get; set; } = 30;
    public int ConnectionIdleLifetime { get; set; } = 300;
    public int ConnectionPruningInterval { get; set; } = 10;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxBatchSize { get; set; } = 100;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public bool EnableDetailedErrors { get; set; } = false;
    public bool EnableQuerySplitting { get; set; } = true;
    public bool EnableConnectionResiliency { get; set; } = true;
}
