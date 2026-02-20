namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Product entity representing clothing items
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Product description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Unique stock keeping unit
    /// </summary>
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Product barcode
    /// </summary>
    public string? Barcode { get; set; }
    
    /// <summary>
    /// Base selling price
    /// </summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Cost price for inventory
    /// </summary>
    public decimal CostPrice { get; set; }
    
    /// <summary>
    /// Product brand
    /// </summary>
    public string? Brand { get; set; }
    
    /// <summary>
    /// Season collection (e.g., "Spring 2024", "Winter 2024")
    /// </summary>
    public string? Season { get; set; }
    
    /// <summary>
    /// Primary material
    /// </summary>
    public string? Material { get; set; }
    
    /// <summary>
    /// Primary color (for products without variations)
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Product category ID
    /// </summary>
    public Guid CategoryId { get; set; }
    
    /// <summary>
    /// Indicates if product is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when product was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when product was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Product category
    /// </summary>
    public virtual Category Category { get; set; } = null!;
    
    /// <summary>
    /// Product variations (sizes, colors, etc.)
    /// </summary>
    public virtual ICollection<ProductVariation> Variations { get; set; } = new List<ProductVariation>();
    
    /// <summary>
    /// Product images
    /// </summary>
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    
    /// <summary>
    /// Inventory records across branches/warehouses
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
