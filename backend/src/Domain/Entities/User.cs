namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// User entity representing a system user
/// </summary>
public class User
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Unique username for login
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email address (unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Hashed password for security
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates if user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Indicates if user email has been confirmed
    /// </summary>
    public bool EmailConfirmed { get; set; } = false;
    
    /// <summary>
    /// Indicates if user phone number has been confirmed
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; } = false;
    
    /// <summary>
    /// Indicates if two-factor authentication is enabled
    /// </summary>
    public bool TwoFactorEnabled { get; set; } = false;
    
    /// <summary>
    /// Indicates if user account is locked out
    /// </summary>
    public bool LockoutEnabled { get; set; } = false;
    
    /// <summary>
    /// Date and time when lockout ends
    /// </summary>
    public DateTime? LockoutEnd { get; set; }
    
    /// <summary>
    /// Number of failed login attempts
    /// </summary>
    public int AccessFailedCount { get; set; } = 0;
    
    /// <summary>
    /// Date and time when user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when user account was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time of last login
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Refresh token for authentication
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Expiration date for refresh token
    /// </summary>
    public DateTime? RefreshTokenExpiry { get; set; }
    
    // Navigation properties
    /// <summary>
    /// User's assigned roles
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    /// <summary>
    /// User's session information
    /// </summary>
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    
    /// <summary>
    /// User's login history
    /// </summary>
    public virtual ICollection<UserLoginHistory> LoginHistory { get; set; } = new List<UserLoginHistory>();
    
    /// <summary>
    /// User's branch assignments
    /// </summary>
    public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
}
