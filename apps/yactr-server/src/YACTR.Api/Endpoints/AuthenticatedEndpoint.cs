using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authorization;
using NodaTime;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Endpoints;

/// <summary>
/// An endpoint that requires the user to be authenticated.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public partial class AuthenticatedEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{
    public override async Task OnBeforeValidateAsync(TRequest req, CancellationToken ct)
    {
        // Failsafe for idiot devs (me).
        if (HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            throw new NotImplementedException("AuthenticatedEndpoint<TRequest, TResponse> cannot be used with AllowAnonymous trait.");
        }

        await Task.CompletedTask;
    }

    // Which means we can guarantee this being populated.
    protected Guid CurrentUserId => Guid.Parse(HttpContext.User.ClaimValue(ClaimTypes.Sid)!);

    protected User CurrentUser() {
        var platformIdentity = HttpContext.User.Identities
            .FirstOrDefault(i => i.AuthenticationType == nameof(YactrAuthenticationType.Platform)) ?? throw new UnauthorizedAccessException("User is not authenticated through the platform.");
        
        return new() {
            Auth0UserId = platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!,
            Email = platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
            Username = platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!,
            LastLogin = Instant.FromUnixTimeMilliseconds(long.Parse(platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationInstant)?.Value!)),
            PlatformPermissions = platformIdentity.Claims.Where(c => c.Type == nameof(PermissionLevel.PlatformPermission)).Select(c => Enum.Parse<Permission>(c.Value!)).ToList(),
            AdminPermissions = platformIdentity.Claims.Where(c => c.Type == nameof(PermissionLevel.AdminPermission)).Select(c => Enum.Parse<Permission>(c.Value!)).ToList(),
            OrganizationUsers = [],
            Organizations = [],
        };
    }

}

/// <summary>
/// An endpoint that requires the user to be authenticated
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TMapper"></typeparam>
public class AuthenticatedEndpoint<TRequest, TResponse, TMapper> : Endpoint<TRequest, TResponse, TMapper>
    where TRequest : notnull
    where TResponse : notnull
    where TMapper : class, IMapper
{
    public override async Task OnBeforeValidateAsync(TRequest req, CancellationToken ct)
    {
        // Failsafe for idiot devs (me).
        if (HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            throw new NotImplementedException("AuthenticatedEndpoint<TRequest, TResponse, TMapper> cannot be used with AllowAnonymous trait.");
        }

        await Task.CompletedTask;
    }

    // Which means we can guarantee this being populated.
    protected Guid CurrentUserId => Guid.Parse(HttpContext.User.ClaimValue(ClaimTypes.Sid)!);

    protected User CurrentUser() {
        var platformIdentity = HttpContext.User.Identities
            .FirstOrDefault(i => i.AuthenticationType == nameof(YactrAuthenticationType.Platform)) ?? throw new UnauthorizedAccessException("User is not authenticated through the platform.");
        
        return new() {
            Auth0UserId = platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!,
            Email = platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
            Username = platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!,
            LastLogin = Instant.FromUnixTimeMilliseconds(long.Parse(platformIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationInstant)?.Value!)),
            PlatformPermissions = platformIdentity.Claims.Where(c => c.Type == nameof(PermissionLevel.PlatformPermission)).Select(c => Enum.Parse<Permission>(c.Value!)).ToList(),
            AdminPermissions = platformIdentity.Claims.Where(c => c.Type == nameof(PermissionLevel.AdminPermission)).Select(c => Enum.Parse<Permission>(c.Value!)).ToList(),
            OrganizationUsers = [],
            Organizations = [],
        };
    }
}