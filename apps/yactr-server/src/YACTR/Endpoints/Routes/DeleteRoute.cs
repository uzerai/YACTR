using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public record DeleteRouteRequest(Guid RouteId);

public class DeleteRoute : AuthenticatedEndpoint<DeleteRouteRequest, EmptyResponse>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Delete("/{RouteId}");
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