namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// User login history entity for tracking authentication attempts
/// </summary>
public class UserLoginHistory
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// User ID who attempted login
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Login provider used (e.g., "Local", "Google", "Microsoft")
    /// </summary>
    public string? LoginProvider { get; set; }
    
    /// <summary>
    /// Provider-specific key (e.g., email, user ID for external providers)
    /// </summary>
    public string? ProviderKey { get; set; }
    
    /// <summary>
    /// IP address of the login attempt
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent string from browser
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// When the login attempt occurred
    /// </summary>
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether the login was successful
    /// </summary>
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// Reason for login failure (if applicable)
    /// </summary>
    public string? FailureReason { get; set; }
    
    // Navigation property back to User
    /// <summary>
    /// The user who attempted login
    /// </summary>
    public virtual User User { get; set; } = null!;
}
