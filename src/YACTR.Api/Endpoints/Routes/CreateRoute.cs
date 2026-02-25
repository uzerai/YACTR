using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class CreateRoute : AuthenticatedEndpoint<RouteRequestData, RouteResponse, RouteDataMapper>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<RoutesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.RoutesWrite)));
    }

    public override async Task HandleAsync(RouteRequestData req, CancellationToken ct)
    {
        var createdRoute = await RouteRepository.CreateAsync(Map.ToEntity(req), ct);

        if (req.Pitches.Length < 1)
        {
            await PitchRepository.CreateAsync(new Pitch()
            {
                Name = req.Name,
                Type = req.Type,
                Height = req.Height,
                Description = req.Description,
                PitchOrder = 0,
                RouteId = createdRoute.Id,
                SectorId = createdRoute.SectorId,
            });
        }
        else
        {
            await Task.WhenAll(req.Pitches.Select(async pitchReq => await PitchRepository.CreateAsync(new Pitch()
            {
                Name = pitchReq.Name,
                Type = pitchReq.Type,
                Description = pitchReq.Description,
                Height = pitchReq.Height,
                PitchOrder = pitchReq.PitchOrder,
                RouteId = createdRoute.Id,
                SectorId = createdRoute.SectorId
            })));
        }

        await Send.CreatedAtAsync<GetRouteById>(createdRoute.Id, await Map.FromEntityAsync(createdRoute, ct), cancellation: ct);
    }
}