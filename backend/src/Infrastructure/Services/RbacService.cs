using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;
using NationalClothingStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NationalClothingStore.Infrastructure.Services;

/// <summary>
/// Role-Based Access Control (RBAC) service implementation
/// </summary>
public class RbacService : IRbacService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly NationalClothingStoreDbContext _dbContext;
    private readonly ILogger<RbacService> _logger;
    private readonly IMemoryCache _cache;

    public RbacService(IUnitOfWork unitOfWork, NationalClothingStoreDbContext dbContext, ILogger<RbacService> logger, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user_permissions_{userId}";
        
        if (!_cache.TryGetValue(cacheKey, out HashSet<string>? userPermissions))
        {
            userPermissions = (await GetUserPermissionsAsync(userId, cancellationToken)).ToHashSet();
            _cache.Set(cacheKey, userPermissions, TimeSpan.FromMinutes(5));
        }

        return userPermissions.Contains(permission);
    }

    public async Task<bool> HasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Any(permission => userPermissions.Contains(permission));
    }

    public async Task<bool> HasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.All(permission => userPermissions.Contains(permission));
    }

    public async Task<bool> HasRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var userRoles = await GetUserRolesAsync(userId, cancellationToken);
        return userRoles.Contains(role);
    }

    public async Task<bool> HasAnyRoleAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var userRoles = await GetUserRolesAsync(userId, cancellationToken);
        return roles.Any(role => userRoles.Contains(role));
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", userId);
            return Enumerable.Empty<string>();
        }

        var permissions = new HashSet<string>();

        // Get user's roles
        var userRoles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var userRole in userRoles)
        {
            // Get permissions for each role
            var rolePermissions = await GetRolePermissionsAsync(userRole.RoleId, cancellationToken);
            foreach (var permission in rolePermissions)
            {
                permissions.Add(permission);
            }
        }

        return permissions;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        return userRoles;
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        // Check if assignment already exists
        var existingAssignment = await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (existingAssignment)
        {
            _logger.LogWarning("User {UserId} already has role {RoleId}", userId, roleId);
            return;
        }

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };

        _dbContext.UserRoles.Add(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Clear cache for this user
        _cache.Remove($"user_permissions_{userId}");
        _cache.Remove($"user_roles_{userId}");

        _logger.LogInformation("Assigned role {RoleId} to user {UserId}", roleId, userId);
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = await _dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (userRole == null)
        {
            _logger.LogWarning("User {UserId} does not have role {RoleId}", userId, roleId);
            return;
        }

        _dbContext.UserRoles.Remove(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Clear cache for this user
        _cache.Remove($"user_permissions_{userId}");
        _cache.Remove($"user_roles_{userId}");

        _logger.LogInformation("Removed role {RoleId} from user {UserId}", roleId, userId);
    }

    public async Task GrantPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        // Check if permission already granted
        var existingPermission = await _dbContext.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);

        if (existingPermission)
        {
            _logger.LogWarning("Role {RoleId} already has permission {PermissionId}", roleId, permissionId);
            return;
        }

        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            GrantedAt = DateTime.UtcNow
        };

        _dbContext.RolePermissions.Add(rolePermission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Clear cache for all users with this role
        await ClearCacheForRoleAsync(roleId);

        _logger.LogInformation("Granted permission {PermissionId} to role {RoleId}", permissionId, roleId);
    }

    public async Task RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermission = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);

        if (rolePermission == null)
        {
            _logger.LogWarning("Role {RoleId} does not have permission {PermissionId}", roleId, permissionId);
            return;
        }

        _dbContext.RolePermissions.Remove(rolePermission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Clear cache for all users with this role
        await ClearCacheForRoleAsync(roleId);

        _logger.LogInformation("Revoked permission {PermissionId} from role {RoleId}", permissionId, roleId);
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission.Name)
            .ToListAsync(cancellationToken);

        return rolePermissions;
    }

    public async Task<IEnumerable<User>> GetUsersWithRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _dbContext.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);
        
        if (!userRoles.Any())
        {
            return Enumerable.Empty<User>();
        }

        return await _dbContext.Users
            .Where(u => userRoles.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetRolesWithPermissionAsync(Guid permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await _dbContext.RolePermissions
            .Where(rp => rp.PermissionId == permissionId)
            .Select(rp => rp.RoleId)
            .ToListAsync(cancellationToken);
        
        if (!rolePermissions.Any())
        {
            return Enumerable.Empty<Role>();
        }

        return await _dbContext.Roles
            .Where(r => rolePermissions.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CanAccessResourceAsync(Guid userId, string resource, string action, CancellationToken cancellationToken = default)
    {
        var permission = $"{resource}.{action}";
        return await HasPermissionAsync(userId, permission, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetEffectivePermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetUserPermissionsAsync(userId, cancellationToken);
    }

    private async Task ClearCacheForRoleAsync(Guid roleId)
    {
        var usersWithRole = await GetUsersWithRoleAsync(roleId, CancellationToken.None);
        
        foreach (var user in usersWithRole)
        {
            _cache.Remove($"user_permissions_{user.Id}");
            _cache.Remove($"user_roles_{user.Id}");
        }
    }
}
