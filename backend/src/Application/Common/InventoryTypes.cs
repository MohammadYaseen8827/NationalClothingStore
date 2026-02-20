namespace NationalClothingStore.Application.Common;

/// <summary>
/// Inventory statistics
/// </summary>
public class InventoryStatistics
{
    public int TotalItems { get; set; }
    public int TotalQuantity { get; set; }
    public int TotalReservedQuantity { get; set; }
    public int TotalAvailableQuantity { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public decimal TotalValue { get; set; }
}

/// <summary>
/// Location inventory value
/// </summary>
public class LocationInventoryValue
{
    public Guid LocationId { get; set; }
    public string LocationType { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public int TotalQuantity { get; set; }
    public int ItemCount { get; set; }
}

/// <summary>
/// Inventory movement
/// </summary>
public class InventoryMovement
{
    public Guid TransactionId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string? ProductVariationSize { get; set; }
    public string? ProductVariationColor { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid? FromBranchId { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
}

/// <summary>
/// Transaction summary
/// </summary>
public class TransactionSummary
{
    public string TransactionType { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageQuantity { get; set; }
    public decimal AverageValue { get; set; }
}

/// <summary>
/// Transaction statistics
/// </summary>
public class TransactionStatistics
{
    public int TotalTransactions { get; set; }
    public int InTransactions { get; set; }
    public int OutTransactions { get; set; }
    public int TransferTransactions { get; set; }
    public int AdjustmentTransactions { get; set; }
    public int TotalQuantityIn { get; set; }
    public int TotalQuantityOut { get; set; }
    public decimal TotalValueIn { get; set; }
    public decimal TotalValueOut { get; set; }
    public decimal NetQuantityChange { get; set; }
    public decimal NetValueChange { get; set; }
    public DateTime FirstTransactionDate { get; set; }
    public DateTime LastTransactionDate { get; set; }
    public Dictionary<string, int> TransactionsByType { get; set; } = new();
    public Dictionary<string, int> TransactionsByDay { get; set; } = new();
}
