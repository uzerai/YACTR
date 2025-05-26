using Microsoft.AspNetCore.Authorization;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.DI.Authorization.Permissions;

public class OrganizationPermissionsAuthorizationHandler : AuthorizationHandler<OrganizationPermissionRequiredAttribute>
{
    private readonly ILogger<OrganizationPermissionsAuthorizationHandler> _logger;
    private readonly IUserContext _userContext;

    public OrganizationPermissionsAuthorizationHandler(
        ILogger<OrganizationPermissionsAuthorizationHandler> logger,
        IUserContext userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationPermissionRequiredAttribute requirement)
    {
        _logger.LogDebug("Handling organization permissions authorization for {Permission}", requirement.Permission);

        var httpContext = context.Resource as HttpContext;
        if (httpContext == null)
        {
            _logger.LogError("HttpContext is null; aborting authorization check.");
            context.Fail();
            return Task.CompletedTask;
        }

        var organizationIdFromRoute = httpContext.GetRouteValue("organizationId") as string;
        if (organizationIdFromRoute == null)
        {
            _logger.LogError("OrganizationPermissionRequiredAttribute assigned on organization-less endpoint; aborting authorization check.");
            context.Fail();
            return Task.CompletedTask;
        }

        if (!Guid.TryParse(organizationIdFromRoute, out var organizationId))
        {
            _logger.LogError("Organization ID from route is not a valid GUID; aborting authorization check.");
            context.Fail();
            return Task.CompletedTask;
        }

        var user = _userContext.CurrentUser;
        if (user == null)
        {
            _logger.LogError("No authenticated user found; aborting authorization check.");
            context.Fail();
            return Task.CompletedTask;
        }

        var organizationPermissions = user.OrganizationUsers
            .Where(ou => ou.OrganizationId == organizationId)
            .SelectMany(ou => ou.Permissions)
            .ToList();

        if (!organizationPermissions.Any())
        {
            _logger.LogError("User has no permissions for organization {OrganizationId}; aborting authorization check.", organizationId);
            context.Fail();
            return Task.CompletedTask;
        }

        if (!organizationPermissions.Any(orgPerm => orgPerm.Permission == requirement.Permission))
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}