namespace NationalClothingStore.Shared;

/// <summary>
/// Low stock alert notification data
/// </summary>
public record LowStockAlertNotification
{
    public Guid InventoryId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public int CurrentQuantity { get; init; }
    public int LowStockThreshold { get; init; }
    public int ReorderPoint { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public DateTime AlertDate { get; init; }
}