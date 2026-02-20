namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// User session entity for tracking active sessions
/// </summary>
public class UserSession
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// User ID who owns this session
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Unique session token
    /// </summary>
    public string SessionToken { get; set; } = string.Empty;
    
    /// <summary>
    /// IP address of the session
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent string from browser
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// When the session started
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the session expires
    /// </summary>
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Indicates if session is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation property back to User
    /// <summary>
    /// The user who owns this session
    /// </summary>
    public virtual User User { get; set; } = null!;
}
