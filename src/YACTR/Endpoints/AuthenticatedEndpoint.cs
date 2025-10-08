using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authorization;

namespace YACTR.Endpoints;

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
            throw new NotImplementedException("AuthenticatedEndpoint<TRequest, TResponse> cannot be used with AllowAnonymous trait.");
        }

        await Task.CompletedTask;
    }

    // Which means we can guarantee this being populated.
    protected Guid CurrentUserId => Guid.Parse(HttpContext.User.ClaimValue(ClaimTypes.Sid)!);
}