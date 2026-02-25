using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.QueryExtensions;
using YACTR.Infrastructure.Database.Repository.Interface;
using Route = YACTR.Domain.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Routes;

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

    public override async Task<Void> HandleAsync(UpdateRouteRequest req, CancellationToken ct)
    {
        var existingRoute = await RouteRepository.BuildTrackedQuery()
            .Where(e => e.Id == req.RouteId)
            .WhereAvailable()
            .Include(e => e.Pitches)
            .FirstOrDefaultAsync(ct);

        if (existingRoute == null)
        {
            return await Send.NotFoundAsync(ct);
        }

        var updatedRoute = Map.UpdateEntity(req.Route, existingRoute);
        await RouteRepository.UpdateAsync(updatedRoute, ct);

        return await Send.NoContentAsync(ct);
    }
}