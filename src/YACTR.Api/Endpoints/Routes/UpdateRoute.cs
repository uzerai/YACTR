using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Domain.Model.Climbing.Topo;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.QueryExtensions;
using Route = YACTR.Domain.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Routes;

public record UpdateRoutePitchRequestData(
    string Name,
    ClimbingType Type,
    Guid? Id,
    int? GearCount,
    string? Description,
    int? Grade,
    int? Height,
    int PitchOrder = 0
);

public record UpdateRouteRequestData(
    Guid SectorId,
    UpdateRoutePitchRequestData[] Pitches,
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

public class UpdateRouteRequest
{
    public Guid RouteId { get; set; }

    [FromBody]
    public required UpdateRouteRequestData Route { get; set; }
};

public class UpdateRoute : AuthenticatedEndpoint<UpdateRouteRequest, EmptyResponse>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Put("/{route_id}");
        Group<RoutesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.RoutesWrite)));
    }

    public override async Task<Void> HandleAsync(UpdateRouteRequest req, CancellationToken ct)
    {
        var existingRoute = await RouteRepository.BuildTrackedQuery()
            .Where(e => e.Id == req.RouteId)
            .WhereAvailable()
            .Include(e => e.Pitches)
            .FirstOrDefaultAsync(ct);

        if (existingRoute == null)
        {
            return await Send.NotFoundAsync(ct);
        }

        existingRoute.Name = req.Route.Name;
        existingRoute.Description = req.Route.Description;
        existingRoute.Grade = req.Route.Grade;
        existingRoute.Type = req.Route.Pitches.Select(x => x.Type)
            .Aggregate(req.Route.Type, (total, next) => total == next ? total : ClimbingType.Mixed);
        existingRoute.BolterName = req.Route.BolterName ?? existingRoute.BolterName;
        existingRoute.FirstAscentClimberName = req.Route.FirstAscentClimberName ?? existingRoute.FirstAscentClimberName;
        existingRoute.FirstAscentDate = req.Route.FirstAscentDate ?? existingRoute.FirstAscentDate;
        existingRoute.TopoImageId = req.Route.TopoImageId ?? existingRoute.TopoImageId;
        existingRoute.TopoLinePoints = req.Route.TopoLinePoints?.ToList() ?? existingRoute.TopoLinePoints;
        existingRoute.TopoImageOverlaySvgId = req.Route.TopoImageOverlayId ?? existingRoute.TopoImageOverlaySvgId;
        existingRoute.SectorId = req.Route.SectorId;
        existingRoute.InSectorOrder = req.Route.InSectorOrder;
        existingRoute.SectorTopoLinePoints = req.Route.SectorTopoLinePoints?.ToList() ?? existingRoute.SectorTopoLinePoints;
        existingRoute.SectorTopoImageOverlaySvgId = req.Route.SectorTopoImageOverlaySvgId;

        if (existingRoute.Pitches.Count > 0)
        {
            existingRoute.Pitches = [.. req.Route.Pitches.Select(p =>
            {
                if (p.Id != null)
                {
                    return new Pitch
                    {
                        Id = p.Id.Value,
                        Type = p.Type,
                        Name = p.Name,
                        Description = p.Description,
                        Height = p.Height,
                        Grade = p.Grade ?? 0,
                        GearCount = p.GearCount,
                        PitchOrder = p.PitchOrder,
                        SectorId = req.Route.SectorId
                    };
                }

                return new Pitch
                {
                    Type = p.Type,
                    Name = p.Name,
                    Description = p.Description,
                    Height = p.Height,
                    Grade = p.Grade ?? 0,
                    GearCount = p.GearCount,
                    PitchOrder = p.PitchOrder,
                    SectorId = req.Route.SectorId
                };
            })];
        }
        else
        {
            existingRoute.Pitches =
            [
                new Pitch
                {
                    Name = req.Route.Name,
                    Type = req.Route.Type,
                    Description = req.Route.Description,
                    Height = req.Route.Height,
                    Grade = req.Route.Grade ?? 0,
                    GearCount = req.Route.GearCount,
                    SectorId = req.Route.SectorId
                }
            ];
        }

        await RouteRepository.UpdateAsync(existingRoute, ct);

        return await Send.NoContentAsync(ct);
    }
}