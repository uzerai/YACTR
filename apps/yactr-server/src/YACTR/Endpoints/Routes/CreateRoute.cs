using FastEndpoints;
using YACTR.Data.Repository.Interface;

using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public class CreateRoute : Endpoint<RouteRequestData, Route>
{
    private readonly IEntityRepository<Route> _routeRepository;

    public CreateRoute(IEntityRepository<Route> routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public override void Configure()
    {
        Post("/");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(RouteRequestData req, CancellationToken ct)
    {
        var createdRoute = await _routeRepository.CreateAsync(new()
        {
            Name = req.Name,
            Description = req.Description,
            Grade = req.Grade,
            SectorId = req.SectorId,
            FirstAscentClimberName = req.FirstAscentClimberName,
            BolterName = req.BolterName,
            Pitches = [],
        }, ct);

        await SendCreatedAtAsync<GetRouteById>(createdRoute.Id, createdRoute, cancellation: ct);
    }
}