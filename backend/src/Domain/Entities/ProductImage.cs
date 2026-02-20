namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// ProductImage entity for storing product images
/// </summary>
public class ProductImage
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Parent product ID
    /// </summary>
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Image URL or file path
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Alternative text for accessibility
    /// </summary>
    public string? AltText { get; set; }
    
    /// <summary>
    /// Image caption or description
    /// </summary>
    public string? Caption { get; set; }
    
    /// <summary>
    /// Sort order for display
    /// </summary>
    public int SortOrder { get; set; } = 0;
    
    /// <summary>
    /// Indicates if this is the primary image
    /// </summary>
    public bool IsPrimary { get; set; } = false;
    
    /// <summary>
    /// Indicates if image is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when image was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when image was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Parent product
    /// </summary>
    public virtual Product Product { get; set; } = null!;
}
