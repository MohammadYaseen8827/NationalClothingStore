namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Role entity for role-based access control
/// </summary>
public class Role
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Role name (unique)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Role description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Indicates if role is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when role was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when role was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Users assigned to this role
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    /// <summary>
    /// Permissions assigned to this role
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
