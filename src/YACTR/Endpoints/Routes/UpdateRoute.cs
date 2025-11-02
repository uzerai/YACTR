using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public class UpdateRouteRequest
{
	public Guid RouteId { get; set; }

	[FromBody]
	public required RouteRequestData Route { get; set; }
};

public class UpdateRoute : AuthenticatedEndpoint<UpdateRouteRequest, EmptyResponse, RouteDataMapper>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Put("/{RouteId}");
        Group<RoutesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.RoutesWrite)));
    }

    public override async Task HandleAsync(UpdateRouteRequest req, CancellationToken ct)
    {
        var existingRoute = await RouteRepository.GetByIdAsync(req.RouteId, ct);
        if (existingRoute == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        Map.UpdateEntity(req.Route, existingRoute);

        if (req.Route.Pitches.Length != 0)
        {
            Logger.LogInformation("Pitches provided in route update request");
        }

        await RouteRepository.UpdateAsync(existingRoute, ct);

        await SendNoContentAsync(ct);
    }
}