using Microsoft.AspNetCore.Authorization;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public class AdminPermissionsAuthorizationHandler(ILogger<AdminPermissionsAuthorizationHandler> logger) : AuthorizationHandler<AdminPermissionRequiredAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPermissionRequiredAttribute requirement)
    {
        logger.LogDebug("Handling admin permissions authorization for {Permission}", requirement.Permission);
        logger.LogDebug("User is {UserId}", context.User.Identity?.Name);
        
        if (context.User.HasClaim(PermissionLevel.AdminPermission.ToString(), requirement.Permission.ToString()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail(new AuthorizationFailureReason(this, $"Authenticated user does not have '{requirement.Permission}' administrative permissions."));
        return Task.CompletedTask;
    }
}