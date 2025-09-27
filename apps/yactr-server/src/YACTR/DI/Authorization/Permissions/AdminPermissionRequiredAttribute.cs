using Microsoft.AspNetCore.Authorization;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public class AdminPermissionRequiredAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public AdminPermissionRequiredAttribute(Permission permission)
    {
        Permission = permission;
    }

    public Permission Permission { get; }

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}