using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.QueryExtensions;
using YACTR.Data.Repository.Interface;

namespace YACTR.DI.Authorization.Permissions;

/// <summary>
/// The UserPermissionsHydrator is what instantiates the custom claims for the local database
/// into the user identity of the ASP.NET authorization flow.
/// <br/>
/// We have 4 levels of permissions:
/// <list type="bullet">
///   <item>
///     <description>
///       AdminPermission
///     </description>
///   </item>
///   <item>
///     <description>
///       PlatformPermission
///     </description>
///   </item>
///   <item>
///     <description>
///       OrganizationPermission
///     </description>
///   </item>
///   <item>
///     <description>
///       OrganizationTeamPermission
///     </description>
///   </item>
/// </list>
/// 
/// <br/>
/// This permission claims transformer will append all levels of permissions to 3 types of identities;
/// <list type="bullet">
///   <item>
///     <description>
///       "Platform": of which there should only ever be one; which contains the LocalClaimTypes.PlatformPermission and LocalClaimTypes.AdminPermission claims
///     </description>
///   </item>
///   <item>
///     <description>
///       "PlatformOrganization": of which there can be multiple; which contains the LocalClaimTypes.OrganizationPermission claims
///     </description> 
///   </item>
///   <item>
///     <description>
///       "PlatformOrganizationTeam": of which there can be multiple; which contains the LocalClaimTypes.OrganizationTeamPermission claims. 
///     </description>
///   </item>
/// </list>
/// </summary>
/// <param name="userRepository"></param>
/// <param name="clock"></param>
/// <param name="logger"></param>
sealed class DatabaseUserPermissionClaimsTransformer(
    IEntityRepository<User> userRepository,
    IClock clock,
    ILogger<DatabaseUserPermissionClaimsTransformer> logger) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // This is going to match the ID of the IDP user.
        string? nameIdentifier = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        ArgumentNullException.ThrowIfNull(nameIdentifier);

        string? email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        string? username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        ArgumentNullException.ThrowIfNull(email);

        User? user = await userRepository
            .BuildReadonlyQuery()
            .WhereAuth0UserId(nameIdentifier)
            .Include(u => u.OrganizationUsers)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            logger.LogWarning("First time login for user {NameIdentifier}. Creating local user.", nameIdentifier);
            user = await userRepository.CreateAsync(new User()
            {
                Auth0UserId = nameIdentifier,
                Email = email,
                Username = username ?? email,
                LastLogin = clock.GetCurrentInstant(),
            });
            logger.LogDebug("Created local user {NameIdentifier}. User: {User}", nameIdentifier, user);
        }
        else
        {
            user.LastLogin = clock.GetCurrentInstant();
            await userRepository.SaveAsync();
        }

        logger.LogInformation("User {NameIdentifier} logged in. User: {User}", nameIdentifier, user);
        ClaimsIdentity localDbUserClaimsIdentity = CreatePlatformClaimsIdentity(user);

        principal.AddIdentity(localDbUserClaimsIdentity);

        logger.LogDebug("Adding organization permissions from {OrganizationUsersCount} organizations", user.OrganizationUsers.Count);
        foreach (var orgUser in user.OrganizationUsers)
        {
            principal.AddIdentity(new ClaimsIdentity(
              [
                new Claim(ClaimTypes.AuthenticationMethod, "PlatformOrganization"),
                new Claim(ClaimTypes.NameIdentifier, orgUser.OrganizationId.ToString()),
                new Claim(ClaimTypes.Role, "User"),
                  .. orgUser.Permissions.Select(
                    permission => new Claim(LocalClaimTypes.OrganizationPermission, permission.ToString()!)
                )], ClaimTypes.AuthenticationMethod, ClaimTypes.NameIdentifier, ClaimTypes.Role));
        }

        return principal;
    }

    private ClaimsIdentity CreatePlatformClaimsIdentity(User user)
    {
        logger.LogDebug("Adding platform identity claims for authenticated user: {UserId}", user.Id);
        ClaimsIdentity results = new([], ClaimTypes.AuthenticationMethod, ClaimTypes.Name, ClaimTypes.Role);

        results.AddClaims([
              new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.AuthenticationMethod, "Platform"),
            new Claim(ClaimTypes.AuthenticationInstant, user.LastLogin.ToUnixTimeMilliseconds().ToString()),
            new Claim(ClaimTypes.Role, user.AdminPermissions.Count != 0 ? "Admin" : "User"),
            .. user.PlatformPermissions.Select(
                permission => new Claim(LocalClaimTypes.PlatformPermission, permission.ToString())
            ),
            .. user.AdminPermissions.Select(
                permission => new Claim(LocalClaimTypes.AdminPermission, permission.ToString())
            )
        ]);

        return results;
    }
}