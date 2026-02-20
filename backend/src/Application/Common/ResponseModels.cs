namespace NationalClothingStore.Application.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public List<ValidationError> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string? RequestId { get; set; }
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

/// <summary>
/// Standard error response for API errors
/// </summary>
public class ErrorResponse
{
    public bool Success { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ValidationError>? ValidationErrors { get; set; }
    public DateTime Timestamp { get; set; }
    public string? RequestId { get; set; }
}

/// <summary>
/// Detailed error response
/// </summary>
public class ErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Target { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Comprehensive error response
/// </summary>
public class ErrorApiResponse : ApiResponse
{
    public ErrorDetail? Error { get; set; }
    public List<ErrorDetail>? Details { get; set; }
    public string? StackTrace { get; set; }
    public string? Path { get; set; }
}

/// <summary>
/// Validation error response
/// </summary>
public class ValidationErrorResponse : ApiResponse
{
    public List<ValidationError> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Problem details response (RFC 7807)
/// </summary>
public class ProblemDetails
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
}

/// <summary>
/// Custom problem details with additional fields
/// </summary>
public class CustomProblemDetails : ProblemDetails
{
    public string? RequestId { get; set; }
    public string? ErrorCode { get; set; }
    public List<ValidationError>? ValidationErrors { get; set; }
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? TraceId { get; set; }
}
