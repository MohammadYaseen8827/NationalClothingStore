namespace NationalClothingStore.Application.Common;

/// <summary>
/// Base exception for application-specific errors
/// </summary>
public abstract class BaseException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    protected BaseException(string errorCode, string message, int statusCode = 500) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    protected BaseException(string errorCode, string message, Exception innerException, int statusCode = 500) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

/// <summary>
/// Validation exception for input validation errors
/// </summary>
public class ValidationException : BaseException
{
    public List<ValidationError> ValidationErrors { get; }

    public ValidationException(string message, List<ValidationError>? validationErrors = null) 
        : base("VALIDATION_ERROR", message, 400)
    {
        ValidationErrors = validationErrors ?? [];
    }

    public ValidationException(string message, string fieldName, string errorMessage) 
        : base("VALIDATION_ERROR", message, 400)
    {
        ValidationErrors = [new ValidationError { Field = fieldName, Message = errorMessage }];
    }
}

/// <summary>
/// Business logic exception for domain rule violations
/// </summary>
public class BusinessException : BaseException
{
    public BusinessException(string errorCode, string message) 
        : base(errorCode, message, 400)
    {
    }

    public BusinessException(string errorCode, string message, Exception innerException) 
        : base(errorCode, message, innerException, 400)
    {
    }
}

/// <summary>
/// Resource not found exception
/// </summary>
public class ResourceNotFoundException : BaseException
{
    public string ResourceType { get; }
    public string ResourceId { get; }

    public ResourceNotFoundException(string resourceType, string resourceId) 
        : base("RESOURCE_NOT_FOUND", $"{resourceType} with ID '{resourceId}' was not found", 404)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public ResourceNotFoundException(string resourceType, string resourceId, string message) 
        : base("RESOURCE_NOT_FOUND", message, 404)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

/// <summary>
/// Unauthorized access exception
/// </summary>
public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Unauthorized access") 
        : base("UNAUTHORIZED", message, 401)
    {
    }
}

/// <summary>
/// Forbidden access exception
/// </summary>
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = "Access forbidden") 
        : base("FORBIDDEN", message, 403)
    {
    }
}

/// <summary>
/// Conflict exception for resource conflicts
/// </summary>
public class ConflictException : BaseException
{
    public ConflictException(string message) 
        : base("CONFLICT", message, 409)
    {
    }
}

/// <summary>
/// External service exception for third-party service errors
/// </summary>
public class ExternalServiceException : BaseException
{
    public string ServiceName { get; }

    public ExternalServiceException(string serviceName, string message) 
        : base("EXTERNAL_SERVICE_ERROR", $"Error in {serviceName}: {message}", 502)
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, string message, Exception innerException) 
        : base("EXTERNAL_SERVICE_ERROR", $"Error in {serviceName}: {message}", innerException, 502)
    {
        ServiceName = serviceName;
    }
}

/// <summary>
/// Rate limit exceeded exception
/// </summary>
public class RateLimitExceededException : BaseException
{
    public TimeSpan RetryAfter { get; }

    public RateLimitExceededException(TimeSpan retryAfter) 
        : base("RATE_LIMIT_EXCEEDED", "Rate limit exceeded. Please try again later.", 429)
    {
        RetryAfter = retryAfter;
    }
}

/// <summary>
/// Validation error model
/// </summary>
public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
