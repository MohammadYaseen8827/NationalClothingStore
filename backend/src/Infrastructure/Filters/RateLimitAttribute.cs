using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Net;

namespace NationalClothingStore.Infrastructure.Filters;

/// <summary>
/// Rate limiting attribute to prevent API abuse
/// </summary>
public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly IMemoryCache _cache;
    private readonly int _requests;
    private readonly TimeSpan _timeWindow;
    private readonly string _identifier;

    public RateLimitAttribute(int requests = 100, int timeWindowSeconds = 60, string identifier = "default")
    {
        _requests = requests;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
        _identifier = identifier;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
        if (cache == null)
        {
            base.OnActionExecuting(context);
            return;
        }

        var clientId = GetClientIdentifier(context.HttpContext);
        var cacheKey = $"rate_limit_{_identifier}_{clientId}";

        var requestCount = cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _timeWindow;
            return 0;
        });

        if (requestCount >= _requests)
        {
            context.Result = new ContentResult
            {
                Content = $"Rate limit exceeded. Maximum {_requests} requests per {_timeWindow.TotalSeconds} seconds.",
                StatusCode = (int)HttpStatusCode.TooManyRequests
            };
            return;
        }

        cache.Set(cacheKey, requestCount + 1);
        base.OnActionExecuting(context);
    }

    private static string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from claims first
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user_{userId}";
        }

        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return $"ip_{ipAddress}";
        }

        // Ultimate fallback
        return $"anonymous_{Guid.NewGuid()}";
    }
}

/// <summary>
/// Rate limiting for authentication endpoints (more restrictive)
/// </summary>
public class AuthRateLimitAttribute : RateLimitAttribute
{
    public AuthRateLimitAttribute() : base(requests: 5, timeWindowSeconds: 60, identifier: "auth")
    {
    }
}

/// <summary>
/// Rate limiting for sensitive operations
/// </summary>
public class SensitiveOperationRateLimitAttribute : RateLimitAttribute
{
    public SensitiveOperationRateLimitAttribute() : base(requests: 10, timeWindowSeconds: 60, identifier: "sensitive")
    {
    }
}
