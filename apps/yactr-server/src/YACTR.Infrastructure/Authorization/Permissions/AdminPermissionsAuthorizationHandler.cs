using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Infrastructure.Authorization.Permissions;

public class AdminPermissionsAuthorizationHandler(ILogger<AdminPermissionsAuthorizationHandler> logger) : AuthorizationHandler<AdminPermissionRequiredAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPermissionRequiredAttribute requirement)
    {
        logger.LogDebug("Handling admin permissions authorization for {Permission}", requirement.Permission);
        logger.LogDebug("User is {UserId}", context.User.Identity?.Name);
        
        if (context.User.HasClaim(nameof(PermissionLevel.AdminPermission), requirement.Permission.ToString()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail(new AuthorizationFailureReason(this, $"Authenticated user does not have '{requirement.Permission}' administrative permissions."));
        return Task.CompletedTask;
    }
}