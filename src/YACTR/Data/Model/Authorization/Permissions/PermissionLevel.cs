namespace YACTR.Data.Model.Authorization.Permissions;

/// <summary>
/// The levels of permissions that are available to users on the platform.
/// </summary>
public enum PermissionLevel
{
    AdminPermission,
    PlatformPermission,
    OrganizationPermission,
    OrganizationTeamPermission
}