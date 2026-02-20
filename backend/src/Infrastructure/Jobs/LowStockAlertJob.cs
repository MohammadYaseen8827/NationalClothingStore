using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Shared;

namespace NationalClothingStore.Infrastructure.Jobs;

/// <summary>
/// Background job for checking low stock alerts and sending notifications
/// </summary>
public class LowStockAlertJob : IBackgroundJob
{
    private readonly ILogger<LowStockAlertJob> _logger;
    private readonly IInventoryManagementService _inventoryManagementService;
    private readonly INotificationService _notificationService;

    public LowStockAlertJob(
        ILogger<LowStockAlertJob> logger,
        IInventoryManagementService inventoryManagementService,
        INotificationService notificationService)
    {
        _logger = logger;
        _inventoryManagementService = inventoryManagementService;
        _notificationService = notificationService;
    }

    public async Task ExecuteAsync(JobExecutionContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting low stock alert job at {Time}", DateTime.UtcNow);

        try
        {
            // Get all low stock alerts
            var lowStockAlerts = await _inventoryManagementService.GetLowStockAlertsAsync(null, null,cancellationToken);

            // Process each alert
            foreach (var alert in lowStockAlerts)
            {
                // Skip already resolved alerts
                if (alert.IsResolved)
                {
                    _logger.LogInformation("Skipping resolved alert for inventory {InventoryId}", alert.InventoryId);
                    continue;
                }

                // Check if alert needs to be processed (not sent in last 24 hours)
                var timeSinceLastNotification = DateTime.UtcNow - alert.AlertDate;
                if (timeSinceLastNotification.TotalHours < 24)
                {
                    await ProcessLowStockAlertAsync(alert, cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Alert for inventory {InventoryId} was already processed in the last 24 hours", alert.InventoryId);
                }
            }

            _logger.LogInformation("Low stock alert job completed successfully. Processed {Count} alerts.", lowStockAlerts.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in low stock alert job");
            throw;
        }
    }

    private async Task ProcessLowStockAlertAsync(LowStockAlert alert, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing low stock alert for inventory {InventoryId}", alert.InventoryId);

        try
        {
            // Send notification (this would integrate with email, SMS, push notifications, etc.)
            await _notificationService.SendLowStockAlertAsync(new LowStockAlertNotification
            {
                InventoryId = alert.InventoryId,
                ProductName = alert.ProductName,
                ProductSku = alert.ProductSKU,
                CurrentQuantity = alert.CurrentQuantity,
                LowStockThreshold = alert.LowStockThreshold,
                ReorderPoint = alert.ReorderPoint,
                BranchName = alert.LocationName,
                AlertDate = alert.AlertDate
            }, cancellationToken);

            // Mark alert as resolved
            await _inventoryManagementService.ResolveLowStockAlertAsync(alert.InventoryId, cancellationToken);

            _logger.LogInformation("Low stock alert processed and notification sent for inventory {InventoryId}", alert.InventoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing low stock alert for inventory {InventoryId}", alert.InventoryId, ex);
        }
    }
}


