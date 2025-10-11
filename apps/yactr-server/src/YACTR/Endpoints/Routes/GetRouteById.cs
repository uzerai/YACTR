using FastEndpoints;
using YACTR.Data.Repository.Interface;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public record GetRouteByIdRequest(Guid RouteId);

public class GetRouteById : Endpoint<GetRouteByIdRequest, RouteResponse, RouteDataMapper>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Get("/{RouteId}");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetRouteByIdRequest req, CancellationToken ct)
    {
        var route = await RouteRepository.GetByIdAsync(req.RouteId, ct);

        if (route == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(await Map.FromEntityAsync(route, ct), cancellation: ct);
    }
}