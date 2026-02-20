namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// InventoryTransaction entity for tracking inventory movements
/// </summary>
public class InventoryTransaction
{
    public Guid Id { get; set; }
    public Guid InventoryId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // IN, OUT, TRANSFER, ADJUSTMENT
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid? FromBranchId { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Inventory Inventory { get; set; } = null!;
    public virtual Branch? FromBranch { get; set; }
    public virtual Branch? ToBranch { get; set; }
    public virtual Warehouse? FromWarehouse { get; set; }
    public virtual Warehouse? ToWarehouse { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;
}
