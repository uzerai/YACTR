using System.Linq;
using Microsoft.AspNetCore.Authorization;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public class PlatformPermissionsAuthorizationHandler : AuthorizationHandler<PlatformPermissionRequiredAttribute>
{
    private readonly ILogger<PlatformPermissionsAuthorizationHandler> _logger;

    public PlatformPermissionsAuthorizationHandler(
        ILogger<PlatformPermissionsAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlatformPermissionRequiredAttribute requirement)
    {
        _logger.LogDebug("Handling platform permissions authorization for {Permission}", requirement.Permission);
        _logger.LogDebug("User is {UserId}", context.User.Identity?.Name);
        if (context.User.HasClaim(LocalClaimTypes.PlatformPermission, requirement.Permission.ToString()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}