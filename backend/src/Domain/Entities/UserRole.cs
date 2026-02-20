namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// User-Role junction entity for many-to-many relationship
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
