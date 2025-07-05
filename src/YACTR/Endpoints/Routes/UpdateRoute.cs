using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;
using YACTR.DTO.RequestData;
using Route = YACTR.Data.Model.Location.Route;

namespace YACTR.Endpoints;

public record UpdateRouteRequest(Guid RouteId);

public class UpdateRoute : Endpoint<UpdateRouteRequest, EmptyResponse>
{
    private readonly IEntityRepository<Route> _routeRepository;

    public UpdateRoute(IEntityRepository<Route> routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public override void Configure()
    {
        Put("/{RouteId}");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateRouteRequest req, CancellationToken ct)
    {
        var existingRoute = await _routeRepository.GetByIdAsync(req.RouteId, ct);
        if (existingRoute == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Note: This would need to be implemented with proper request body handling
        // The original controller had incomplete update logic
        await _routeRepository.UpdateAsync(existingRoute, ct);
        
        await SendNoContentAsync(ct);
    }
} 