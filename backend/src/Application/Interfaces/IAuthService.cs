using System.Security.Claims;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Authentication service interface for JWT token management
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates user credentials and generates JWT tokens
    /// </summary>
    Task<AuthResponse?> AuthenticateAsync(LoginRequest request);

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    Task RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// Checks if a token is blacklisted
    /// </summary>
    Task<bool> IsTokenBlacklistedAsync(string tokenId);

    /// <summary>
    /// Blacklists a token
    /// </summary>
    Task BlacklistTokenAsync(string tokenId);

    /// <summary>
    /// Validates a JWT token and returns the user principal
    /// </summary>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);

    /// <summary>
    /// Gets the current user from the claims principal
    /// </summary>
    Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal);
}

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

/// <summary>
/// Authentication response model
/// </summary>
public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public User User { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
}
