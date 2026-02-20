using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using NationalClothingStore.Application.Common;

namespace NationalClothingStore.Infrastructure.Middleware;

/// <summary>
/// Global error handling middleware
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = exception switch
        {
            ValidationException validationEx => await HandleValidationExceptionAsync(context, validationEx),
            ResourceNotFoundException notFoundEx => await HandleNotFoundExceptionAsync(context, notFoundEx),
            UnauthorizedException unauthorizedEx => await HandleUnauthorizedExceptionAsync(context, unauthorizedEx),
            ForbiddenException forbiddenEx => await HandleForbiddenExceptionAsync(context, forbiddenEx),
            ConflictException conflictEx => await HandleConflictExceptionAsync(context, conflictEx),
            RateLimitExceededException rateLimitEx => await HandleRateLimitExceptionAsync(context, rateLimitEx),
            ExternalServiceException externalEx => await HandleExternalServiceExceptionAsync(context, externalEx),
            BusinessException businessEx => await HandleBusinessExceptionAsync(context, businessEx),
            BaseException baseEx => await HandleBaseExceptionAsync(context, baseEx),
            _ => await HandleGenericExceptionAsync(context, exception)
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static async Task<ErrorResponse> HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = 400;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            ValidationErrors = exception.ValidationErrors,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleNotFoundExceptionAsync(HttpContext context, ResourceNotFoundException exception)
    {
        context.Response.StatusCode = 404;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedException exception)
    {
        context.Response.StatusCode = 401;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleForbiddenExceptionAsync(HttpContext context, ForbiddenException exception)
    {
        context.Response.StatusCode = 403;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleConflictExceptionAsync(HttpContext context, ConflictException exception)
    {
        context.Response.StatusCode = 409;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleRateLimitExceptionAsync(HttpContext context, RateLimitExceededException exception)
    {
        context.Response.StatusCode = 429;
        context.Response.Headers.Append("Retry-After", exception.RetryAfter.TotalSeconds.ToString());
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleExternalServiceExceptionAsync(HttpContext context, ExternalServiceException exception)
    {
        context.Response.StatusCode = 502;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleBusinessExceptionAsync(HttpContext context, BusinessException exception)
    {
        context.Response.StatusCode = 400;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleBaseExceptionAsync(HttpContext context, BaseException exception)
    {
        context.Response.StatusCode = exception.StatusCode;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }

    private static async Task<ErrorResponse> HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = 500;
        return new ErrorResponse
        {
            Success = false,
            ErrorCode = "INTERNAL_SERVER_ERROR",
            Message = "An internal server error occurred",
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };
    }
}
