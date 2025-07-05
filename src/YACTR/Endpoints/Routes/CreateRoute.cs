using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;
using YACTR.DTO.RequestData;
using Route = YACTR.Data.Model.Location.Route;

namespace YACTR.Endpoints;

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
            Pitches = [.. req.Pitches.Select(p => new Pitch
            {
                Name = p.Name,
                Description = p.Description,
                Type = p.Type,
                SectorId = req.SectorId,
            })],
        }, ct);

        await SendCreatedAtAsync<GetRouteById>(createdRoute.Id, createdRoute, cancellation: ct);
    }
} 