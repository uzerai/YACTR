using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Domain.Model.Climbing.Topo;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Service;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public record CreateRoutePitchRequest(
    string Name,
    ClimbingType Type,
    Guid? Id,
    int? GearCount,
    string? Description,
    int? Grade,
    int? Height,
    int PitchOrder = 0
);

public record CreateRouteRequest(
    Guid SectorId,
    CreateRoutePitchRequest[] Pitches,
    string Name,
    ClimbingType Type,
    int InSectorOrder = 0,
    int? GearCount = null,
    int? Height = 0,
    Guid? SectorTopoImageId = null,
    Guid? SectorTopoImageOverlaySvgId = null,
    Guid? TopoImageId = null,
    Guid? TopoImageOverlayId = null,
    string? Description = null,
    int? Grade = null,
    string? FirstAscentClimberName = null,
    Instant? FirstAscentDate = null,
    string? BolterName = null,
    TopoLinePoint[]? TopoLinePoints = default,
    TopoLinePoint[]? SectorTopoLinePoints = default
);

public record CreateRoutePitchResponse(
    Guid Id,
    string? Name,
    int PitchOrder,
    ClimbingType Type,
    string? Description,
    int? Grade,
    int? GearCount,
    int? Height
);

public record CreateRouteResponse(
    Guid Id,
    string Name,
    string? Description,
    ClimbingType Type,
    Guid SectorId,
    Instant CreatedAt,
    Instant UpdatedAt,
    int InSectorOrder,
    int? Grade = null,
    int? GearCount = null,
    int? Height = null,
    Instant? FirstAscentDate = null,
    string? FirstAscentClimberName = null,
    string? BolterName = null,
    Guid? TopoImageId = null,
    string? TopoImageUrl = null,
    Guid? TopoImageOverlayId = null,
    string? TopoImageOverlayUrl = null,
    Guid? SectorTopoImageId = null,
    string? SectorTopoImageUrl = null,
    Guid? SectorTopoImageOverlayId = null,
    string? SectorTopoImageOverlayUrl = null,
    CreateRoutePitchResponse[]? Pitches = default,
    TopoLinePoint[]? TopoLinePoints = default,
    TopoLinePoint[]? SectorTopoLinePoints = default
);

public class CreateRoute : AuthenticatedEndpoint<CreateRouteRequest, CreateRouteResponse>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }
    public required IEntityRepository<Pitch> PitchRepository { get; init; }
    public required IImageStorageService ImageStorageService { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<RoutesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.RoutesWrite)));
    }

    public override async Task HandleAsync(CreateRouteRequest req, CancellationToken ct)
    {
        var routeType = req.Pitches.Select(x => x.Type).Aggregate(req.Type, (total, next) => total == next ? total : ClimbingType.Mixed);
        var newRoute = new Route
        {
            Name = req.Name,
            Type = routeType,
            Description = req.Description,
            Grade = req.Grade,
            FirstAscentClimberName = req.FirstAscentClimberName,
            FirstAscentDate = req.FirstAscentDate,
            BolterName = req.BolterName,
            InSectorOrder = req.InSectorOrder,
            SectorId = req.SectorId,
            TopoImageId = req.TopoImageId,
            TopoImageOverlaySvgId = req.TopoImageOverlayId,
            SectorTopoImageId = req.SectorTopoImageId,
            SectorTopoImageOverlaySvgId = req.SectorTopoImageOverlaySvgId,
            TopoLinePoints = req.TopoLinePoints?.ToList(),
            SectorTopoLinePoints = req.SectorTopoLinePoints?.ToList()
        };

        var createdRoute = await RouteRepository.CreateAsync(newRoute, ct);

        if (req.Pitches.Length < 1)
        {
            await PitchRepository.CreateAsync(new Pitch()
            {
                Name = req.Name,
                Type = req.Type,
                Height = req.Height,
                GearCount = req.GearCount,
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
                GearCount = pitchReq.GearCount,
                PitchOrder = pitchReq.PitchOrder,
                RouteId = createdRoute.Id,
                SectorId = createdRoute.SectorId
            })));
        }

        await Send.CreatedAtAsync<GetRouteById>(createdRoute.Id, await MapRouteToResponseAsync(createdRoute, ct), cancellation: ct);
    }

    private async Task<CreateRouteResponse> MapRouteToResponseAsync(Route route, CancellationToken ct)
    {
        return new CreateRouteResponse(
            route.Id,
            route.Name,
            route.Description,
            route.Type,
            route.SectorId,
            route.CreatedAt,
            route.UpdatedAt,
            route.InSectorOrder,
            route.Grade,
            route.Pitches.Sum(p => p.GearCount),
            route.Pitches.Sum(p => p.Height),
            route.FirstAscentDate,
            route.FirstAscentClimberName,
            route.BolterName,
            route.TopoImageId,
            route.TopoImageId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.TopoImageId.Value, ct) : null,
            route.TopoImageOverlaySvgId,
            route.TopoImageOverlaySvgId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.TopoImageOverlaySvgId.Value, ct) : null,
            route.SectorTopoImageId,
            route.SectorTopoImageId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.SectorTopoImageId.Value, ct) : null,
            route.SectorTopoImageOverlaySvgId,
            route.SectorTopoImageOverlaySvgId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.SectorTopoImageOverlaySvgId.Value, ct) : null,
            route.Pitches.Select(p => new CreateRoutePitchResponse(p.Id, p.Name, p.PitchOrder, p.Type, p.Description, p.Grade, p.GearCount, p.Height)).ToArray(),
            route.TopoLinePoints?.ToArray(),
            route.SectorTopoLinePoints?.ToArray()
        );
    }
}