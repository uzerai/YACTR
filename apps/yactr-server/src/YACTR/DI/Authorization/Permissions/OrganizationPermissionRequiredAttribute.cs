using Microsoft.AspNetCore.Authorization;
using YACTR.Model.Authorization.Permissions;

namespace YACTR.DI.Authorization.Permissions;

public class OrganizationPermissionRequiredAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public OrganizationPermissionRequiredAttribute(Permission permission)
    {
        Permission = permission;
    }

    public Permission Permission { get; }

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}