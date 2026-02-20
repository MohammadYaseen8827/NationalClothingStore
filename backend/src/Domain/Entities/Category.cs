namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Category entity for product categorization
/// </summary>
public class Category
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Category description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Unique category code
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Parent category ID for hierarchical structure
    /// </summary>
    public Guid? ParentCategoryId { get; set; }
    
    /// <summary>
    /// Sort order for display
    /// </summary>
    public int SortOrder { get; set; } = 0;
    
    /// <summary>
    /// Indicates if category is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when category was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when category was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Parent category reference
    /// </summary>
    public virtual Category? ParentCategory { get; set; }
    
    /// <summary>
    /// Child categories collection
    /// </summary>
    public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();
    
    /// <summary>
    /// Products in this category
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
