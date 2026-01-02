using Microsoft.AspNetCore.Authorization;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public class PlatformPermissionsAuthorizationHandler(ILogger<PlatformPermissionsAuthorizationHandler> logger) : AuthorizationHandler<PlatformPermissionRequiredAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlatformPermissionRequiredAttribute requirement)
    {
        logger.LogDebug("Handling platform permissions authorization for {Permission}", requirement.Permission);
        logger.LogDebug("User is {UserId}", context.User.Identity?.Name);
        if (context.User.HasClaim(PermissionLevel.PlatformPermission.ToString(), requirement.Permission.ToString()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail(new AuthorizationFailureReason(this, $"Authenticated user does not have '{requirement.Permission}' platform permissions."));
        return Task.CompletedTask;
    }
}