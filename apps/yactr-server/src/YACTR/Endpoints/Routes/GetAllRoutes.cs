using FastEndpoints;
using YACTR.Data.Repository.Interface;

using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public class GetAllRoutes : Endpoint<EmptyRequest, List<Route>>
{
    private readonly IEntityRepository<Route> _routeRepository;

    public GetAllRoutes(IEntityRepository<Route> routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var routes = await _routeRepository.GetAllAsync(ct);
        await SendAsync([.. routes], cancellation: ct);
    }
}