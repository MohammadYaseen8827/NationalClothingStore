using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NationalClothingStore.Infrastructure.Jobs;

public class DataArchivalJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataArchivalJob> _logger;

    public DataArchivalJob(IServiceProvider serviceProvider, ILogger<DataArchivalJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DataArchivalJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                // Run archival daily at 2 AM UTC
                if (now.Hour == 2 && now.Minute < 1)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var archivalService = scope.ServiceProvider.GetRequiredService<Application.Services.IDataArchivalService>();

                    var cutoff = now.AddYears(-7); // Retain 7 years
                    await archivalService.ArchiveOldDataAsync(cutoff, stoppingToken);
                    await archivalService.PurgeExpiredDataAsync(stoppingToken);

                    var summary = await archivalService.GetArchivalStatusAsync(stoppingToken);
                    _logger.LogInformation("Archival job completed: {Summary}", summary);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during archival job");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("DataArchivalJob stopping.");
    }
}
