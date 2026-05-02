using FastEndpoints;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Infrastructure.Authorization.Permissions;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class DeleteRouteRequest
{
    [BindFrom("route_id")]
    public required Guid RouteId { get; set; }
}

public class DeleteRoute : AuthenticatedEndpoint<DeleteRouteRequest, EmptyResponse>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Delete("/{route_id}");
        Group<RoutesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.RoutesWrite)));
    }

    public override async Task HandleAsync(DeleteRouteRequest req, CancellationToken ct)
    {
        var route = await RouteRepository.GetByIdAsync(req.RouteId, ct);

        if (route == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await RouteRepository.DeleteAsync(route, ct);
        await Send.NoContentAsync(ct);
    }
}