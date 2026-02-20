namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Inventory entity for tracking stock levels
/// </summary>
public class Inventory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual ProductVariation? ProductVariation { get; set; }
    public virtual Branch Branch { get; set; } = null!;
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
}
