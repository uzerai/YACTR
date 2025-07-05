using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;
using Route = YACTR.Data.Model.Location.Route;

namespace YACTR.Endpoints;

public record DeleteRouteRequest(Guid RouteId);

public class DeleteRoute : Endpoint<DeleteRouteRequest, EmptyResponse>
{
    private readonly IEntityRepository<Route> _routeRepository;

    public DeleteRoute(IEntityRepository<Route> routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public override void Configure()
    {
        Delete("/{RouteId}");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteRouteRequest req, CancellationToken ct)
    {
        var route = await _routeRepository.GetByIdAsync(req.RouteId, ct);
        
        if (route == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await _routeRepository.DeleteAsync(route, ct);
        await SendNoContentAsync(ct);
    }
} 