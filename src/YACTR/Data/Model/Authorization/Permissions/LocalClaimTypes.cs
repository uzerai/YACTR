namespace YACTR.Data.Model.Authorization.Permissions;

/// <summary>
/// Custom claim types for local identities that are pulled from the database.
/// Related exclusively to Permissions for the time being.
/// </summary>
public static class LocalClaimTypes
{
    public static string PlatformPermission = "PlatformPermission";
    public static string AdminPermission = "AdminPermission";
    public static string OrganizationPermission = "OrganizationPermission";
    public static string OrganizationTeamPermission = "OrganizationTeamPermission";
}