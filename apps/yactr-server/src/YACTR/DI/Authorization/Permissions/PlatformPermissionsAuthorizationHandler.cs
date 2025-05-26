using Microsoft.AspNetCore.Authorization;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.DI.Authorization.Permissions;

public class PlatformPermissionsAuthorizationHandler : AuthorizationHandler<PlatformPermissionRequiredAttribute>
{
    private readonly ILogger<PlatformPermissionsAuthorizationHandler> _logger;
    private readonly IUserContext _userContext;

    public PlatformPermissionsAuthorizationHandler(
        ILogger<PlatformPermissionsAuthorizationHandler> logger,
        IUserContext userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlatformPermissionRequiredAttribute requirement)
    {
        _logger.LogDebug("Handling platform permissions authorization for {Permission}", requirement.Permission);

        var user = _userContext.CurrentUser;
        if (user == null)
        {
            _logger.LogError("No authenticated user found; aborting authorization check.");
            context.Fail();
            return Task.CompletedTask;
        }

        var platformPermissions = user.PlatformPermissions.ToList();

        if (!platformPermissions.Any())
        {
            _logger.LogError("User has no platform permissions; aborting authorization check.");
            context.Fail();
            return Task.CompletedTask;
        }

        if (!platformPermissions.Any(perm => perm == requirement.Permission))
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