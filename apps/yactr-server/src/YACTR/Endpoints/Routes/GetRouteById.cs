using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;
using Route = YACTR.Data.Model.Location.Route;

namespace YACTR.Endpoints;

public record GetRouteByIdRequest(Guid RouteId);

public class GetRouteById : Endpoint<GetRouteByIdRequest, Route>
{
    private readonly IEntityRepository<Route> _routeRepository;

    public GetRouteById(IEntityRepository<Route> routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public override void Configure()
    {
        Get("/{RouteId}");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetRouteByIdRequest req, CancellationToken ct)
    {
        var route = await _routeRepository.GetByIdAsync(req.RouteId, ct);
        
        if (route == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(route, cancellation: ct);
    }
} 