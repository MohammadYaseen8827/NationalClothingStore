namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Branch entity representing physical store locations
/// </summary>
public class Branch
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Branch name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique branch code
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Branch address
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Branch city
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Branch country
    /// </summary>
    public string Country { get; set; } = string.Empty;
    
    /// <summary>
    /// Branch phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Branch email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Indicates if branch is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when branch was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when branch was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Users assigned to this branch
    /// </summary>
    public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
    
    /// <summary>
    /// Inventory records for this branch
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
