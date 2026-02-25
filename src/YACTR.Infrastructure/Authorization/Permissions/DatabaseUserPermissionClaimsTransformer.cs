using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NodaTime;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Infrastructure.Database.QueryExtensions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Infrastructure.Authorization.Permissions;

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
/// </list>
/// </summary>
/// <param name="userRepository"></param>
/// <param name="clock"></param>
/// <param name="logger"></param>
public sealed class DatabaseUserPermissionClaimsTransformer(
    IEntityRepository<User> userRepository,
    IClock clock,
    IMemoryCache transformerCache,
    ILogger<DatabaseUserPermissionClaimsTransformer> logger) : IClaimsTransformation
{
    private static string TransformerCacheKey(string nameIdentifier) => $"DatabaseUserPermissionClaimsTransformer:{nameIdentifier}";

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        using (logger.BeginScope("Transforming claims for user {NameIdentifier}", principal.Identity?.Name))
        {
            // This is going to match the ID of the IDP user.
            var nameIdentifier = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            ArgumentNullException.ThrowIfNull(nameIdentifier);

            logger.LogInformation("User {NameIdentifier} token used.", nameIdentifier);
            if (transformerCache.TryGetValue(TransformerCacheKey(nameIdentifier), out ClaimsPrincipal? cachedPrincipal))
            {
                if (cachedPrincipal != null)
                {
                    logger.LogDebug("Returning cached claims for user {NameIdentifier}", nameIdentifier);
                    return cachedPrincipal;
                }
            }

            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            ArgumentNullException.ThrowIfNull(email);

            var user = await userRepository
                .BuildReadonlyQuery()
                .WhereAuth0UserId(nameIdentifier)
                .Include(u => u.OrganizationUsers)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                logger.LogInformation("First time login for user {NameIdentifier}. Creating local user.", nameIdentifier);
                user = await userRepository.CreateAsync(new User()
                {
                    Auth0UserId = nameIdentifier,
                    Email = email,
                    Username = username ?? email,
                    LastLogin = clock.GetCurrentInstant(),
                });
                logger.LogDebug("Created local user {NameIdentifier}.", nameIdentifier);
            }
            else
            {
                user.LastLogin = clock.GetCurrentInstant();
                await userRepository.SaveAsync();
            }

            var localDbUserClaimsIdentity = CreatePlatformClaimsIdentity(user);

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
                        permission => new Claim(PermissionLevel.OrganizationPermission.ToString(), permission.ToString()!)
                    )], ClaimTypes.AuthenticationMethod, ClaimTypes.NameIdentifier, ClaimTypes.Role));
            }

            transformerCache.Set(TransformerCacheKey(nameIdentifier), principal, TimeSpan.FromMinutes(1));

            return principal;
        }
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
                permission => new Claim(PermissionLevel.PlatformPermission.ToString(), permission.ToString())
            ),
            .. user.AdminPermissions.Select(
                permission => new Claim(PermissionLevel.AdminPermission.ToString(), permission.ToString())
            )
        ]);

        return results;
    }
}