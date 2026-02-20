using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace NationalClothingStore.Infrastructure.Filters;

/// <summary>
/// Custom authorization attribute for checking user permissions
/// </summary>
public class AuthorizePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _permissions;

    public AuthorizePermissionAttribute(params string[] permissions)
    {
        _permissions = permissions ?? Array.Empty<string>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Skip authorization if action is decorated with [AllowAnonymous]
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
        {
            return;
        }

        var user = context.HttpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_permissions.Length == 0)
        {
            return; // Only require authentication, no specific permissions
        }

        var userPermissions = GetUserPermissions(user);
        
        if (!_permissions.Any(permission => userPermissions.Contains(permission, StringComparer.OrdinalIgnoreCase)))
        {
            context.Result = new ForbidResult();
        }
    }

    private static IEnumerable<string> GetUserPermissions(ClaimsPrincipal user)
    {
        return user.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();
    }
}

/// <summary>
/// Custom authorization attribute for checking user roles
/// </summary>
public class AuthorizeRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public AuthorizeRoleAttribute(params string[] roles)
    {
        _roles = roles ?? Array.Empty<string>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Skip authorization if action is decorated with [AllowAnonymous]
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
        {
            return;
        }

        var user = context.HttpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_roles.Length == 0)
        {
            return; // Only require authentication, no specific roles
        }

        var userRoles = GetUserRoles(user);
        
        if (!_roles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase)))
        {
            context.Result = new ForbidResult();
        }
    }

    private static IEnumerable<string> GetUserRoles(ClaimsPrincipal user)
    {
        return user.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .ToList();
    }
}
