using Microsoft.AspNetCore.Authorization;
using YACTR.Data.Model.Authorization.Permissions;
namespace YACTR.DI.Authorization.Permissions;

public class OrganizationPermissionsAuthorizationHandler : AuthorizationHandler<OrganizationPermissionRequiredAttribute>
{
    private readonly ILogger<OrganizationPermissionsAuthorizationHandler> _logger;

    public OrganizationPermissionsAuthorizationHandler(
        ILogger<OrganizationPermissionsAuthorizationHandler> logger)
    {
        _logger = logger;
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

        string? organizationIdFromRoute = httpContext.GetRouteValue("OrganizationId")?.ToString();
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

        if (context.User.Identities.First(x => x.Name == organizationId.ToString())
            .HasClaim(LocalClaimTypes.OrganizationPermission, requirement.Permission.ToString()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}