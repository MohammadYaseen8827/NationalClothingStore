namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// RolePermission junction entity for many-to-many relationship between Role and Permission
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime GrantedAt { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
