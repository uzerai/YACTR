using Microsoft.AspNetCore.Authorization;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public class AdminPermissionsAuthorizationHandler : AuthorizationHandler<AdminPermissionRequiredAttribute>
{
    private readonly ILogger<AdminPermissionsAuthorizationHandler> _logger;

    public AdminPermissionsAuthorizationHandler(
        ILogger<AdminPermissionsAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPermissionRequiredAttribute requirement)
    {
        _logger.LogDebug("Handling admin permissions authorization for {Permission}", requirement.Permission);
        _logger.LogDebug("User is {UserId}", context.User.Identity?.Name);
        if (context.User.HasClaim(LocalClaimTypes.AdminPermission, requirement.Permission.ToString()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}