using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public abstract class AdminPermissionBypassAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement, IAuthorizationRequirementData
{
    protected bool HasAdminPermissionInstead(AuthorizationHandlerContext context, TRequirement requirementData)
    {
        try {
            var permission = requirementData.GetRequirements().TryGetPropertyValue<Permission>("Permission");

            if (context.User.HasClaim(PermissionLevel.AdminPermission.ToString(), permission.ToString()))
            {
                return true;
            }
        } catch { /* Do nothing, requirement does not have permission property value */ }

        return false;
    }
}