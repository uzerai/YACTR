using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public class CreateRoute : AuthenticatedEndpoint<RouteRequestData, RouteResponse, RouteDataMapper>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<RoutesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.RoutesWrite)));
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
            pitches = req.Pitches!.Select((reqDataPitch, Index) => new Pitch()
            {
                Name = reqDataPitch.Name,
                Description = reqDataPitch.Description,
                Type = reqDataPitch.Type,
                SectorId = req.SectorId,
                PitchOrder = Index
            });
        }

        var createdRoute = await RouteRepository.CreateAsync(new()
        {
            Name = req.Name,
            Description = req.Description,
            Type = req.Type,
            Grade = req.Grade,
            SectorId = req.SectorId,
            FirstAscentClimberName = req.FirstAscentClimberName,
            BolterName = req.BolterName,
            TopoImageId = req.TopoImageId,
            TopoImageOverlaySvgId = req.TopoImageOverlayId,
            SectorTopoImageId = req.SectorTopoImageId,
            SectorTopoImageOverlaySvgId = req.SectorTopoImageOverlaySvgId,
            Pitches = pitches.ToList(),
        }, ct);

        await SendCreatedAtAsync<GetRouteById>(createdRoute.Id, await Map.FromEntityAsync(createdRoute, ct), cancellation: ct);
    }
}