namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// UserBranch junction entity for many-to-many relationship between User and Branch
/// </summary>
public class UserBranch
{
    public Guid UserId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
}
