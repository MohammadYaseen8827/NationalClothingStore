namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// PurchaseOrderItem entity for individual items in a purchase order
/// </summary>
public class PurchaseOrderItem
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public int ReceivedQuantity { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductVariation? ProductVariation { get; set; }
}
