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

public class UpdateRoute : AuthenticatedEndpoint<UpdateRouteRequest, EmptyResponse>
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

        existingRoute.Name = req.Route.Name;
        existingRoute.Description = req.Route.Description;
        existingRoute.Grade = req.Route.Grade;
        existingRoute.Type = req.Route.Type;
        existingRoute.SectorId = req.Route.SectorId;
        existingRoute.TopoImageId = req.Route.TopoImageId;
        existingRoute.BolterName = req.Route.BolterName;
        existingRoute.FirstAscentClimberName = req.Route.FirstAscentClimberName;
        existingRoute.FirstAscentDate = req.Route.FirstAscentDate;

        if (req.Route.Pitches.Length != 0)
        {
            Logger.LogInformation("Pitches provided in route update request");
        }

        await RouteRepository.UpdateAsync(existingRoute, ct);

        await SendNoContentAsync(ct);
    }
}