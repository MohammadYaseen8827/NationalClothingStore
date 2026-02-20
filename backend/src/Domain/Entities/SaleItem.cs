namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// SaleItem entity for individual items in a sale
/// </summary>
public class SaleItem
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Sale Sale { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductVariation? ProductVariation { get; set; }
}
