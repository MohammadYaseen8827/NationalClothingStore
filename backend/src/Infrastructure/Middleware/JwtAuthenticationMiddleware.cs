using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Http.StatusCodes;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.Infrastructure.Middleware;

/// <summary>
/// JWT Authentication Middleware for validating JWT tokens
/// </summary>
public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;
    private readonly JwtSettings _jwtSettings;

    public JwtAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<JwtAuthenticationMiddleware> logger,
        IOptions<JwtSettings> jwtSettings)
    {
        _next = next;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var token = ExtractTokenFromRequest(context);
        
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claimsPrincipal = ValidateToken(token);
                if (claimsPrincipal != null)
                {
                    context.User = claimsPrincipal;
                    
                    // Check if token is blacklisted
                    var tokenId = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Jti);
                    if (!string.IsNullOrEmpty(tokenId) && await authService.IsTokenBlacklistedAsync(tokenId))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token has been revoked");
                        return;
                    }
                }
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token has expired");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has expired");
                return;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                _logger.LogWarning("Invalid token signature");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token signature");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token");
                return;
            }
        }

        await _next(context);
    }

    private string? ExtractTokenFromRequest(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Also check for token in cookies
        return context.Request.Cookies["access_token"];
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(_jwtSettings.SecretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
    }
}

/// <summary>
/// JWT Settings configuration
/// </summary>
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}
