using System.ComponentModel.DataAnnotations;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Domain.Interface.Repository;
using YACTR.Infrastructure.Database.QueryExtensions;

using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public record GetRouteByIdRequest(Guid RouteId);

public class GetRouteById([Required] IEntityRepository<Route> routeRepository) : Endpoint<GetRouteByIdRequest, RouteResponse, RouteDataMapper>
{
    public override void Configure()
    {
        Get("/{route_id}");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetRouteByIdRequest req, CancellationToken ct)
    {
        var route = await routeRepository.BuildReadonlyQuery()
            .Where(e => e.Id == req.RouteId)
            .WhereAvailable()
            .Include(e => e.Pitches.Where(x => x.DeletedAt == null))
            .Include(e => e.TopoImage)
            .Include(e => e.TopoImageOverlaySvg)
            .Include(e => e.SectorTopoImage)
            .Include(e => e.SectorTopoImageOverlaySvg)
            .FirstOrDefaultAsync(ct);

        if (route == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(await Map.FromEntityAsync(route, ct), cancellation: ct);
    }
}