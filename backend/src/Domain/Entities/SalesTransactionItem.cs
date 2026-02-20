using System.ComponentModel.DataAnnotations;

namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Represents an item in a sales transaction
/// </summary>
public class SalesTransactionItem
{
    /// <summary>
    /// Unique identifier for the transaction item
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Reference to the sales transaction
    /// </summary>
    [Required]
    public Guid SalesTransactionId { get; set; }
    
    /// <summary>
    /// Reference to the product
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Reference to the product variation (nullable for products without variations)
    /// </summary>
    public Guid? ProductVariationId { get; set; }
    
    /// <summary>
    /// Reference to the inventory record
    /// </summary>
    [Required]
    public Guid InventoryId { get; set; }
    
    /// <summary>
    /// Quantity of items
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
    
    /// <summary>
    /// Unit price at time of sale
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; } = 0;
    
    /// <summary>
    /// Discount amount for this item
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; } = 0;
    
    /// <summary>
    /// Tax amount for this item
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; } = 0;
    
    /// <summary>
    /// Total price for this item (quantity × unit price - discount + tax)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalPrice { get; set; } = 0;
    
    /// <summary>
    /// Serial numbers for tracked items (comma-separated)
    /// </summary>
    [StringLength(1000)]
    public string? SerialNumbers { get; set; }
    
    /// <summary>
    /// Notes about this item
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Date and time when item was added to transaction
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Parent sales transaction
    /// </summary>
    public virtual SalesTransaction SalesTransaction { get; set; } = null!;
    
    /// <summary>
    /// Product sold
    /// </summary>
    public virtual Product Product { get; set; } = null!;
    
    /// <summary>
    /// Product variation (if applicable)
    /// </summary>
    public virtual ProductVariation? ProductVariation { get; set; }
    
    /// <summary>
    /// Inventory record
    /// </summary>
    public virtual Inventory Inventory { get; set; } = null!;
    
    // Computed properties
    /// <summary>
    /// Gets the subtotal for this item (quantity × unit price)
    /// </summary>
    public decimal Subtotal => Quantity * UnitPrice;
    
    /// <summary>
    /// Gets the final price after discount
    /// </summary>
    public decimal PriceAfterDiscount => Math.Max(0, Subtotal - DiscountAmount);
}