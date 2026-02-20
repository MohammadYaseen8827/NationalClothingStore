using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.Application.Jobs;

/// <summary>
/// Background job for cleaning up old audit events
/// </summary>
public class AuditCleanupJob : IBackgroundJob
{
    private readonly ILogger<AuditCleanupJob> _logger;
    private readonly IAuditService _auditService;

    public AuditCleanupJob(ILogger<AuditCleanupJob> logger, IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    public async Task ExecuteAsync(JobExecutionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting audit cleanup job");

            // Get cleanup settings from job data
            var retentionDays = context.JobData.TryGetValue("RetentionDays", out var days) ? (int)days : 90;
            var beforeDate = DateTime.UtcNow.AddDays(-retentionDays);

            // Archive old audit events
            await _auditService.ArchiveAuditEventsAsync(beforeDate, cancellationToken);

            // Delete very old audit events (older than 1 year)
            var deleteBeforeDate = DateTime.UtcNow.AddDays(-365);
            await _auditService.DeleteAuditEventsAsync(deleteBeforeDate, cancellationToken);

            _logger.LogInformation("Completed audit cleanup job - removed events older than {RetentionDays} days", retentionDays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute audit cleanup job");
            throw;
        }
    }
}
