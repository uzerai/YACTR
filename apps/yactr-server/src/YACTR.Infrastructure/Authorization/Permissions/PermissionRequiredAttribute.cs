using Microsoft.AspNetCore.Authorization;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Infrastructure.Authorization.Permissions;

// Keeping all implementations of the PermissionRequiredAttribute here for now; since there's only three (with a planned four).

public class AdminPermissionRequiredAttribute(Permission permission) : PermissionRequiredAttribute(PermissionLevel.AdminPermission, permission);
public class PlatformPermissionRequiredAttribute(Permission permission) : PermissionRequiredAttribute(PermissionLevel.PlatformPermission, permission);
public class OrganizationPermissionRequiredAttribute(Permission permission) : PermissionRequiredAttribute(PermissionLevel.OrganizationPermission, permission);

public abstract class PermissionRequiredAttribute(PermissionLevel permissionLevel, Permission permission) : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public readonly Permission Permission = permission;
    public readonly PermissionLevel PermissionLevel = permissionLevel;
    
    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}