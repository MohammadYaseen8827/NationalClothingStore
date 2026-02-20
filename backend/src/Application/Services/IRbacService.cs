using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Role-Based Access Control (RBAC) service interface
/// </summary>
public interface IRbacService
{
    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has any of the specified permissions
    /// </summary>
    Task<bool> HasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has all of the specified permissions
    /// </summary>
    Task<bool> HasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    Task<bool> HasRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has any of the specified roles
    /// </summary>
    Task<bool> HasAnyRoleAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a user
    /// </summary>
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles for a user
    /// </summary>
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Grants a permission to a role
    /// </summary>
    Task GrantPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a permission from a role
    /// </summary>
    Task RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a role
    /// </summary>
    Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with a specific role
    /// </summary>
    Task<IEnumerable<User>> GetUsersWithRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles with a specific permission
    /// </summary>
    Task<IEnumerable<Role>> GetRolesWithPermissionAsync(Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user can access a specific resource
    /// </summary>
    Task<bool> CanAccessResourceAsync(Guid userId, string resource, string action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets effective permissions for a user (including inherited from roles)
    /// </summary>
    Task<IEnumerable<string>> GetEffectivePermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
