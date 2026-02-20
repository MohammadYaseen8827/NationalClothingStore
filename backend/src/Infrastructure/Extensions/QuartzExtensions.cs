using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Infrastructure.Services;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.Infrastructure.Extensions;

/// <summary>
/// Quartz.NET dependency injection extensions
/// </summary>
public static class QuartzExtensions
{
    /// <summary>
    /// Adds Quartz.NET services with custom configuration
    /// </summary>
    public static IServiceCollection AddQuartzBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseInMemoryStore();
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        // Register the background job service
        services.AddSingleton<IBackgroundJobService, BackgroundJobService>();

        return services;
    }

    /// <summary>
    /// Configures Quartz.NET with database persistence
    /// </summary>
    public static IServiceCollection AddQuartzBackgroundJobsWithDatabase(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            // In production, you would use persistent job store:
            // q.UseSqlServer(connectionString);
            // q.UseJsonSerializer();
            q.UseInMemoryStore();
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        // Register the background job service
        services.AddSingleton<IBackgroundJobService, BackgroundJobService>();

        return services;
    }

    /// <summary>
    /// Adds a job type to the DI container
    /// </summary>
    public static IServiceCollection AddBackgroundJob<T>(this IServiceCollection services) where T : class, IBackgroundJob
    {
        services.AddTransient<T>();
        return services;
    }
}
