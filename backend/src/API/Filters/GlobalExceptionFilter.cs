using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NationalClothingStore.API.Controllers;

namespace NationalClothingStore.API.Filters;

/// <summary>
/// Global exception handling filter
/// </summary>
public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An unhandled exception occurred");

        var response = CreateErrorResponse(context.Exception);
        
        context.Result = new ObjectResult(response)
        {
            StatusCode = GetStatusCode(context.Exception)
        };

        context.ExceptionHandled = true;
    }

    private static ApiResponse CreateErrorResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentException => new ApiResponse
            {
                Success = false,
                Message = "Invalid argument provided",
                Errors = [exception.Message],
                Timestamp = DateTime.UtcNow
            },
            UnauthorizedAccessException => new ApiResponse
            {
                Success = false,
                Message = "Unauthorized access",
                Errors = [exception.Message],
                Timestamp = DateTime.UtcNow
            },
            KeyNotFoundException => new ApiResponse
            {
                Success = false,
                Message = "Resource not found",
                Errors = [exception.Message],
                Timestamp = DateTime.UtcNow
            },
            InvalidOperationException => new ApiResponse
            {
                Success = false,
                Message = "Invalid operation",
                Errors = [exception.Message],
                Timestamp = DateTime.UtcNow
            },
            _ => new ApiResponse
            {
                Success = false,
                Message = "An internal server error occurred",
                Errors = [exception.Message],
                Timestamp = DateTime.UtcNow
            }
        };
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
