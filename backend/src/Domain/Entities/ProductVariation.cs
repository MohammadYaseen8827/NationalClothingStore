namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// ProductVariation entity representing different sizes and colors of products
/// </summary>
public class ProductVariation
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Parent product ID
    /// </summary>
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Size (e.g., "S", "M", "L", "XL", "32", "34")
    /// </summary>
    public string Size { get; set; } = string.Empty;
    
    /// <summary>
    /// Color (e.g., "Red", "Blue", "Black", "White")
    /// </summary>
    public string Color { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique SKU for this variation
    /// </summary>
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional price adjustment from base product price
    /// </summary>
    public decimal AdditionalPrice { get; set; } = 0.00m;
    
    /// <summary>
    /// Cost price for this specific variation
    /// </summary>
    public decimal CostPrice { get; set; }
    
    /// <summary>
    /// Stock quantity for this variation
    /// </summary>
    public int StockQuantity { get; set; } = 0;
    
    /// <summary>
    /// Indicates if variation is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when variation was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when variation was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Parent product
    /// </summary>
    public virtual Product Product { get; set; } = null!;
    
    /// <summary>
    /// Inventory records for this variation
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
