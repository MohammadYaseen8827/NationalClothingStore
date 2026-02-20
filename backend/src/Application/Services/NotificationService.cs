using Microsoft.Extensions.Logging;
using NationalClothingStore.Shared;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Simple notification service implementation
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendLowStockAlertAsync(LowStockAlertNotification notification,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending low stock alert notification for inventory {InventoryId} - {ProductName}", 
            notification.InventoryId, notification.ProductName);

        // In a real implementation, this would send email, SMS, push notifications, etc.
        // For now, we'll just log the notification
        _logger.LogInformation("Low stock alert notification sent: InventoryId={InventoryId}, Product={ProductName}, Quantity={CurrentQuantity}, Threshold={LowStockThreshold}", 
            notification.InventoryId, notification.ProductName, notification.CurrentQuantity, notification.LowStockThreshold);

        await Task.CompletedTask;
    }
}
