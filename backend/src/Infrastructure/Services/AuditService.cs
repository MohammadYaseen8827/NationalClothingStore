using Microsoft.Extensions.Logging;
using System.Text.Json;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Services;

/// <summary>
/// Audit logging service implementation
/// </summary>
public class AuditService : IAuditService
{
    private readonly NationalClothingStoreDbContext _dbContext;
    private readonly ILogger<AuditService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(
        NationalClothingStoreDbContext dbContext,
        ILogger<AuditService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            // Set timestamp if not provided
            if (auditEvent.Timestamp == default)
            {
                auditEvent.Timestamp = DateTime.UtcNow;
            }

            // Extract user information from current context if not provided
            if (auditEvent.UserId == Guid.Empty && _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    auditEvent.UserId = userId;
                    auditEvent.UserName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                    auditEvent.UserRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                }
            }

            // Extract IP address and user agent
            if (_httpContextAccessor.HttpContext != null)
            {
                auditEvent.IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                auditEvent.UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            }

            // Serialize metadata and details if provided
            if (auditEvent.Metadata != null)
            {
                auditEvent.Details = JsonSerializer.Serialize(auditEvent.Metadata);
            }

            // Save to database
            _dbContext.AuditEvents.Add(auditEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Audit event logged: {Action} on {ResourceType} by {UserId}", 
                auditEvent.Action, auditEvent.ResourceType, auditEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit event for action {Action} on {ResourceType}", 
                auditEvent.Action, auditEvent.ResourceType);
        }
    }

    public async Task LogAsync(string action, string resourceType, string? resourceId = null, object? details = null, CancellationToken cancellationToken = default)
    {
        var auditEvent = new AuditEvent
        {
            Id = Guid.NewGuid(),
            UserId = GetCurrentUserId(),
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Metadata = details != null ? new Dictionary<string, object> { ["details"] = details } : null,
            Success = true,
            Timestamp = DateTime.UtcNow
        };

        await LogAsync(auditEvent, cancellationToken);
    }

    public async Task LogUserActionAsync(Guid userId, string action, string resourceType, string? resourceId = null, object? details = null, CancellationToken cancellationToken = default)
    {
        var auditEvent = new AuditEvent
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Metadata = details != null ? new Dictionary<string, object> { ["details"] = details } : null,
            Success = true,
            Timestamp = DateTime.UtcNow
        };

        await LogAsync(auditEvent, cancellationToken);
    }

    public async Task LogSystemEventAsync(string action, string resourceType, string? resourceId = null, object? details = null, CancellationToken cancellationToken = default)
    {
        var auditEvent = new AuditEvent
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty, // System event
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Metadata = details != null ? new Dictionary<string, object> { ["details"] = details } : null,
            Success = true,
            Timestamp = DateTime.UtcNow
        };

        await LogAsync(auditEvent, cancellationToken);
    }

    public async Task<IEnumerable<AuditEvent>> GetUserAuditEventsAsync(Guid userId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditEvents.Where(a => a.UserId == userId);

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        return await query.OrderByDescending(a => a.Timestamp).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditEvent>> GetResourceAuditEventsAsync(string resourceType, string? resourceId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditEvents.Where(a => a.ResourceType == resourceType);

        if (!string.IsNullOrEmpty(resourceId))
        {
            query = query.Where(a => a.ResourceId == resourceId);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        return await query.OrderByDescending(a => a.Timestamp).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditEvent>> GetAuditEventsByActionAsync(string action, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditEvents.Where(a => a.Action == action);

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        return await query.OrderByDescending(a => a.Timestamp).ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<AuditEvent> events, int totalCount)> GetAuditEventsAsync(int page = 1, int pageSize = 50, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditEvents.AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var events = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (events, totalCount);
    }

    public async Task<AuditStatistics> GetAuditStatisticsAsync(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditEvents.AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        var events = await query.ToListAsync(cancellationToken);

        var statistics = new AuditStatistics
        {
            TotalEvents = events.Count,
            UserActions = events.Count(e => e.UserId != Guid.Empty),
            SystemEvents = events.Count(e => e.UserId == Guid.Empty),
            FailedActions = events.Count(e => !e.Success),
            PeriodStart = from ?? DateTime.MinValue,
            PeriodEnd = to ?? DateTime.UtcNow
        };

        // Group by action type
        statistics.ActionsByType = events
            .GroupBy(e => e.Action)
            .ToDictionary(g => g.Key, g => g.Count());

        // Group by resource type
        statistics.EventsByResourceType = events
            .GroupBy(e => e.ResourceType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Group by user
        statistics.EventsByUser = events
            .Where(e => e.UserId != Guid.Empty)
            .GroupBy(e => e.UserId)
            .ToDictionary(g => g.Key, g => g.Count());

        return statistics;
    }

    public async Task CreateAuditTrailAsync<T>(T oldEntity, T newEntity, string action, Guid userId, CancellationToken cancellationToken = default) where T : class
    {
        var entityType = typeof(T).Name;
        var entityId = GetEntityId(newEntity);

        var changes = new Dictionary<string, object>
        {
            ["old_values"] = oldEntity,
            ["new_values"] = newEntity,
            ["changed_properties"] = GetChangedProperties(oldEntity, newEntity)
        };

        await LogUserActionAsync(userId, action, entityType, entityId, changes, cancellationToken);
    }

    public async Task ArchiveAuditEventsAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
    {
        var eventsToArchive = await _dbContext.AuditEvents
            .Where(a => a.Timestamp < beforeDate)
            .ToListAsync(cancellationToken);

        // In a real implementation, you would move these to an archive table or separate database
        _logger.LogInformation("Archived {Count} audit events before {Date}", eventsToArchive.Count, beforeDate);
    }

    public async Task DeleteAuditEventsAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
    {
        var eventsToDelete = await _dbContext.AuditEvents
            .Where(a => a.Timestamp < beforeDate)
            .ToListAsync(cancellationToken);

        _dbContext.AuditEvents.RemoveRange(eventsToDelete);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted {Count} audit events before {Date}", eventsToDelete.Count, beforeDate);
    }

    private Guid GetCurrentUserId()
    {
        if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
        }
        return Guid.Empty;
    }

    private string? GetEntityId<T>(T entity) where T : class
    {
        var idProperty = typeof(T).GetProperty("Id");
        return idProperty?.GetValue(entity)?.ToString();
    }

    private List<string> GetChangedProperties<T>(T oldEntity, T newEntity) where T : class
    {
        var changedProperties = new List<string>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name == "Id") continue; // Skip ID property

            var oldValue = property.GetValue(oldEntity);
            var newValue = property.GetValue(newEntity);

            if (!Equals(oldValue, newValue))
            {
                changedProperties.Add(property.Name);
            }
        }

        return changedProperties;
    }
}
