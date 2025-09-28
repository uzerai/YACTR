using FastEndpoints;
using YACTR.Data.Model.Climbing;
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
        IEnumerable<Pitch> pitches = [];
        if (req.Pitches?.Count() < 1)
        {
            pitches.Append(
                new Pitch()
                {
                    Name = req.Name,
                    Description = req.Description,
                    Type = req.Type,
                    SectorId = req.SectorId,
                });
        }
        else
        {
            pitches = req.Pitches.Select(reqDataPitch => new Pitch()
            {
                Name = reqDataPitch.Name,
                Description = reqDataPitch.Description,
                Type = reqDataPitch.Type,
                SectorId = req.SectorId
            });
        }

        var createdRoute = await _routeRepository.CreateAsync(new()
        {
            Name = req.Name,
            Description = req.Description,
            Grade = req.Grade,
            SectorId = req.SectorId,
            FirstAscentClimberName = req.FirstAscentClimberName,
            BolterName = req.BolterName,
            Pitches = pitches.ToList(),
        }, ct);

        await SendCreatedAtAsync<GetRouteById>(createdRoute.Id, createdRoute, cancellation: ct);
    }
}