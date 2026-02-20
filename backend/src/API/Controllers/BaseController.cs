using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Base API controller with common functionality
/// </summary>
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseController(ILogger<BaseController> logger) : ControllerBase
{
    /// <summary>
    /// Gets the current user ID from claims
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? 
                          User.FindFirst("sub") ?? 
                          User.FindFirst("user_id");
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in claims");
        }

        return userId;
    }

    /// <summary>
    /// Gets the current user's email from claims
    /// </summary>
    protected string GetCurrentUserEmail()
    {
        var emailClaim = User.FindFirst(ClaimTypes.Email) ?? 
                        User.FindFirst("email");
        
        return emailClaim?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the current user's roles from claims
    /// </summary>
    protected IEnumerable<string> GetCurrentUserRoles()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Gets the current user's permissions from claims
    /// </summary>
    protected IEnumerable<string> GetCurrentUserPermissions()
    {
        return User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Checks if the current user has a specific permission
    /// </summary>
    protected bool HasPermission(string permission)
    {
        return GetCurrentUserPermissions()
            .Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    protected bool HasRole(string role)
    {
        return GetCurrentUserRoles()
            .Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a successful result with data
    /// </summary>
    protected IActionResult Success<T>(T data, string message = "Operation completed successfully")
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Returns a successful result without data
    /// </summary>
    protected IActionResult Success(string message = "Operation completed successfully")
    {
        var response = new ApiResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Returns a paginated result
    /// </summary>
    protected IActionResult PaginatedResult<T>(IEnumerable<T> data, int totalCount, int page, int pageSize, string message = "Data retrieved successfully")
    {
        var response = new PaginatedApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Returns a validation error result
    /// </summary>
    protected IActionResult ValidationError(string message, ModelStateDictionary? modelState = null)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = modelState?.SelectMany(kvp => kvp.Value.Errors.Select(e => $"{kvp.Key}: {e.ErrorMessage}")).ToList() ?? new List<string>(),
            Timestamp = DateTime.UtcNow
        };

        return BadRequest(response);
    }
    /// <summary>
    /// Returns a not found result
    /// </summary>
    protected IActionResult NotFoundResult(string message = "Resource not found")
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        return NotFound(response);
    }

    /// <summary>
    /// Returns an unauthorized result
    /// </summary>
    protected IActionResult UnauthorizedResult(string message = "Unauthorized access")
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        return Unauthorized(response);
    }

    /// <summary>
    /// Returns a forbidden result
    /// </summary>
    protected IActionResult ForbiddenResult(string message = "Access forbidden")
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        return Forbid();
    }

    /// <summary>
    /// Returns a server error result
    /// </summary>
    protected IActionResult ServerErrorResult(string message = "An internal server error occurred")
    {
        logger.LogError("Server error: {Message}", message);
        
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        return StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    /// <summary>
    /// Handles exceptions and returns appropriate error response
    /// </summary>
    protected IActionResult HandleException(Exception ex, string message = "An error occurred while processing your request")
    {
        logger.LogError(ex, "Exception occurred: {Message}", message);

        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = new List<string> { ex.Message },
            Timestamp = DateTime.UtcNow
        };

        return ex switch
        {
            ArgumentException => BadRequest(response),
            UnauthorizedAccessException => Unauthorized(response),
            KeyNotFoundException => NotFound(response),
            InvalidOperationException => BadRequest(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }
}

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// API response wrapper with data
/// </summary>
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

/// <summary>
/// Paginated API response wrapper
/// </summary>
public class PaginatedApiResponse<T> : ApiResponse
{
    public IEnumerable<T>? Data { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
