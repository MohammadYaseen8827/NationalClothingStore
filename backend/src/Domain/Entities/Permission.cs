namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Permission entity for role-based access control
/// </summary>
public class Permission
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Permission name (unique)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Permission description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Permission category for grouping
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Indicates if permission is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when permission was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when permission was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Roles that have this permission
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
