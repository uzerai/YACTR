using Microsoft.AspNetCore.Authorization;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public class OrganizationPermissionsAuthorizationHandler(ILogger<OrganizationPermissionsAuthorizationHandler> logger) : AuthorizationHandler<OrganizationPermissionRequiredAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationPermissionRequiredAttribute requirement)
    {
        logger.LogDebug("Handling organization permissions authorization for {Permission}", requirement.Permission);

        if (context.Resource is not HttpContext httpContext)
        {
            // This is an implementation error, as this handler should not be used for non-HttpContext based resources.
            throw new NotImplementedException("Context Resource is not HttpContext type.");
        }

        // This permission handler does not work without an OrganizationId route parameter present in the httpContext.
        string? organizationIdFromRoute = httpContext.GetRouteValue("OrganizationId")?.ToString()
            ?? throw new NotImplementedException("No OrganizationId route parameter found.");

        if (!Guid.TryParse(organizationIdFromRoute, out var organizationId))
        {
            // This is a valid use-case to simply deny authorization for enumeration attacks without runtime exception.
            logger.LogWarning("Organization ID from route is not a valid GUID; aborting authorization check.");
            context.Fail();
            return Task.CompletedTask;
        }

        var organizationIdentity = context.User.Identities.FirstOrDefault(x => x.Name == organizationId.ToString());
        var hasOrganizationClaim = organizationIdentity?.HasClaim(LocalClaimTypes.OrganizationPermission, requirement.Permission.ToString()) ?? false;

        if (hasOrganizationClaim)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}