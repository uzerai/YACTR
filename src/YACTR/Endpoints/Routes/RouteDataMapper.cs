using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Model.Climbing.Topo;
using YACTR.DI.Service;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public record RoutePitchRequestData(
    string Name,
    ClimbingType Type,
    string? Description,
    string? Grade);

public record RouteRequestData(
    Guid SectorId,
    RoutePitchRequestData[] Pitches,
    string Name,
    ClimbingType Type,
    Guid? SectorTopoImageId = null,
    Guid? SectorTopoImageOverlaySvgId = null,
    Guid? TopoImageId = null,
    Guid? TopoImageOverlayId = null,
    string? Description = null,
    string? Grade = null,
    string? FirstAscentClimberName = null,
    Instant? FirstAscentDate = null,
    string? BolterName = null,
    TopoLinePoint[]? TopoLinePoints = default,
    TopoLinePoint[]? SectorTopoLinePoints = default
);

public record RouteResponse(
    Guid Id,
    string Name,
    string? Description,
    ClimbingType Type,
    Guid SectorId,
    string? Grade = null,
    Instant? FirstAscentDate = null,
    string? FirstAscentClimberName = null,
    string? BolterName = null,
    Guid? TopoImageId = null,
    string? TopoImageUrl = null,
    Guid? TopoImageOverlayId = null,
    string? TopoImageOverlayUrl = null,
    Guid? SectorTopoImageId = null,
    string? SectorTopoImageUrl = null,
    string? SectorTopoImageOverlayUrl = null,
    Pitch[]? Pitches = default,
    TopoLinePoint[]? TopoLinePoints = default,
    TopoLinePoint[]? SectorTopoLinePoints = default
);

public class RouteDataMapper : Mapper<RouteRequestData, RouteResponse, Route>
{
    public override Route ToEntity(RouteRequestData r) => new Route()
    {
        Name = r.Name,
        Type = r.Type,
        Description = r.Description,
        Grade = r.Grade,
        FirstAscentClimberName = r.FirstAscentClimberName,
        FirstAscentDate = r.FirstAscentDate,
        BolterName = r.BolterName,
        SectorId = r.SectorId,
        Pitches = r.Pitches.Select(p => new Pitch()
        {
            Name = p.Name,
            Type = p.Type,
            SectorId = r.SectorId,
            Description = p.Description,
        }).ToList(),
        TopoImageId = r.TopoImageId,
        TopoImageOverlaySvgId = r.TopoImageOverlayId,
        SectorTopoImageId = r.SectorTopoImageId,
        SectorTopoImageOverlaySvgId = r.SectorTopoImageOverlaySvgId,
        TopoLinePoints = r.TopoLinePoints?.ToList(),
        SectorTopoLinePoints = r.SectorTopoLinePoints?.ToList()
    };

    public override Route UpdateEntity(RouteRequestData r, Route e)
    {
        e.Name = r.Name;
        e.Description = r.Description;
        e.Grade = r.Grade;
        e.Type = r.Type;

        e.BolterName = r.BolterName ?? e.BolterName;
        e.FirstAscentClimberName = r.FirstAscentClimberName ?? e.FirstAscentClimberName;
        e.FirstAscentDate = r.FirstAscentDate ?? e.FirstAscentDate;

        e.TopoImageId = r.TopoImageId ?? e.TopoImageId;
        e.TopoLinePoints = r.TopoLinePoints?.ToList() ?? e.TopoLinePoints;
        e.TopoImageOverlaySvgId = r.TopoImageOverlayId ?? e.TopoImageOverlaySvgId;

        e.SectorId = r.SectorId;
        e.SectorTopoLinePoints = r.SectorTopoLinePoints?.ToList() ?? e.SectorTopoLinePoints;
        e.SectorTopoImageOverlaySvgId = r.SectorTopoImageOverlaySvgId ?? e.TopoImageOverlaySvgId;

        return e;
    }

    public override async Task<RouteResponse> FromEntityAsync(Route e, CancellationToken ct = default)
    {
        using var mappingScope = CreateScope();
        IImageStorageService service = mappingScope.Resolve<IImageStorageService>();

        return new(
            e.Id,
            e.Name,
            e.Description,
            e.Type,
            e.SectorId,
            e.Grade,
            e.FirstAscentDate,
            e.FirstAscentClimberName,
            e.BolterName,
            e.TopoImageId,
            e.TopoImageId.HasValue ? await service.GetImageUrlAsync(e.TopoImageId.Value, ct) : null,
            e.TopoImageOverlaySvgId,
            e.TopoImageOverlaySvgId.HasValue ? await service.GetImageUrlAsync(e.TopoImageOverlaySvgId.Value, ct) : null,
            e.SectorTopoImageId,
            e.SectorTopoImageId.HasValue ? await service.GetImageUrlAsync(e.SectorTopoImageId.Value, ct) : null,
            e.SectorTopoImageOverlaySvgId.HasValue ? await service.GetImageUrlAsync(e.SectorTopoImageOverlaySvgId.Value, ct) : null,
            [],
            e.TopoLinePoints?.ToArray(),
            e.SectorTopoLinePoints?.ToArray()
        );
    }
}