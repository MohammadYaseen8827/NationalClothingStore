using NationalClothingStore.Domain.Entities;
using NationalClothingStore.Application.Common;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Audit logging service interface
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an audit event
    /// </summary>
    Task LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an audit event with automatic user detection
    /// </summary>
    Task LogAsync(string action, string resourceType, string? resourceId = null, object? details = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a user action
    /// </summary>
    Task LogUserActionAsync(Guid userId, string action, string resourceType, string? resourceId = null, object? details = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a system event
    /// </summary>
    Task LogSystemEventAsync(string action, string resourceType, string? resourceId = null, object? details = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit events for a user
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetUserAuditEventsAsync(Guid userId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit events for a resource
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetResourceAuditEventsAsync(string resourceType, string? resourceId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit events by action type
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetAuditEventsByActionAsync(string action, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all audit events with pagination
    /// </summary>
    Task<(IEnumerable<AuditEvent> events, int totalCount)> GetAuditEventsAsync(int page = 1, int pageSize = 50, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit statistics
    /// </summary>
    Task<AuditStatistics> GetAuditStatisticsAsync(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an audit trail for an entity
    /// </summary>
    Task CreateAuditTrailAsync<T>(T oldEntity, T newEntity, string action, Guid userId, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Archives old audit events
    /// </summary>
    Task ArchiveAuditEventsAsync(DateTime beforeDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes old audit events
    /// </summary>
    Task DeleteAuditEventsAsync(DateTime beforeDate, CancellationToken cancellationToken = default);
}
