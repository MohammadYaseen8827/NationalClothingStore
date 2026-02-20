using NationalClothingStore.Application.Common;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service for handling and logging errors
/// </summary>
public interface IErrorHandlingService
{
    /// <summary>
    /// Logs an error and creates an error response
    /// </summary>
    void LogError(Exception exception, string? context = null);

    /// <summary>
    /// Logs an error with additional context
    /// </summary>
    void LogError(Exception exception, string context, Dictionary<string, object>? additionalData = null);

    /// <summary>
    /// Creates a standardized error response
    /// </summary>
    ErrorResponse CreateErrorResponse(Exception exception, string? requestId = null);

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    ValidationErrorResponse CreateValidationErrorResponse(ValidationException exception, string? requestId = null);

    /// <summary>
    /// Creates a problem details response
    /// </summary>
    CustomProblemDetails CreateProblemDetails(Exception exception, string? requestId = null, string? path = null);

    /// <summary>
    /// Logs a warning
    /// </summary>
    void LogWarning(string message, string? context = null);

    /// <summary>
    /// Logs an informational message
    /// </summary>
    void LogInformation(string message, string? context = null);

    /// <summary>
    /// Logs a debug message
    /// </summary>
    void LogDebug(string message, string? context = null);
}
