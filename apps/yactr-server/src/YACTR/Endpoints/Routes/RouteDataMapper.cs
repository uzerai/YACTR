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
	string? Grade,
	int? Height,
	int PitchOrder = 0
);

public record RouteRequestData(
    Guid SectorId,
    RoutePitchRequestData[] Pitches,
    string Name,
    ClimbingType Type,
    int InSectorOrder = 0,
    int? Height = 0,
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

public record RoutePitchResponse(
    Guid Id,
    string? Name,
    int PitchOrder,
    ClimbingType Type,
    string? Description,
    int? Height
);

public record RouteResponse(
    Guid Id,
    string Name,
    string? Description,
    ClimbingType Type,
    Guid SectorId,
    int InSectorOrder,
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
    RoutePitchResponse[]? Pitches = default,
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
        InSectorOrder = r.InSectorOrder,
        SectorId = r.SectorId,
        Pitches = r.Pitches.Select(p => new Pitch()
        {
            Name = p.Name,
            Type = p.Type,
            Height = p.Height,
            PitchOrder = p.PitchOrder,
            SectorId = r.SectorId, // Pitch should always be in the same sector as the route.
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
        e.InSectorOrder = r.InSectorOrder;
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
            e.InSectorOrder,
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
            e.Pitches.Select(p => new RoutePitchResponse(p.Id, p.Name, p.PitchOrder, p.Type, p.Description, p.Height)).ToArray(),
            e.TopoLinePoints?.ToArray(),
            e.SectorTopoLinePoints?.ToArray()
        );
	}
}