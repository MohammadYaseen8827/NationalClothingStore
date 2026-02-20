namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Warehouse entity representing storage locations
/// </summary>
public class Warehouse
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Warehouse name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique warehouse code
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse address
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse city
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse country
    /// </summary>
    public string Country { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Warehouse email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Indicates if warehouse is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when warehouse was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when warehouse was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Inventory records for this warehouse
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
