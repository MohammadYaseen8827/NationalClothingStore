using NationalClothingStore.Shared;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Interface for sending notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Send low stock alert notification
    /// </summary>
    Task SendLowStockAlertAsync(LowStockAlertNotification notification, CancellationToken cancellationToken = default);
}
