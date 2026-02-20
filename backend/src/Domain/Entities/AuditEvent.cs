namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Audit event entity for tracking system actions
/// </summary>
public class AuditEvent
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string? ResourceId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public string? UserName { get; set; }
    public string? UserRole { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
