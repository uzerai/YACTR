using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Climbing;
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
    string? BolterName = null);

public record RouteResponse(
    Guid Id,
    string Name,
    string? Description,
    ClimbingType Type,
    string? Grade = null,
    Instant? FirstAscentDate = null,
    string? FirstAscentClimberName = null,
    string? BolterName = null,
    Guid? TopoImageId = null,
    string? TopoImageUrl = null,
    string? TopoImageOverlayUrl = null,
    Guid? SectorTopoImageId = null,
    string? SectorTopoImageUrl = null,
    string? SectorTopoImageOverlayUrl = null
);

public class RouteDataMapper : Mapper<RouteRequestData, RouteResponse, Route>
{
    public override async Task<RouteResponse> FromEntityAsync(Route e, CancellationToken ct = default)
    {
        using var mappingScope = CreateScope();
        IImageStorageService service = mappingScope.Resolve<IImageStorageService>();

        return new(
            e.Id,
            e.Name,
            e.Description,
            e.Type,
            e.Grade,
            e.FirstAscentDate,
            e.FirstAscentClimberName,
            e.BolterName,
            e.TopoImageId,
            e.TopoImageId.HasValue ? await service.GetImageUrlAsync(e.TopoImageId.Value, ct) : null,
            e.TopoImageOverlaySvgId.HasValue ? await service.GetImageUrlAsync(e.TopoImageOverlaySvgId.Value, ct) : null,
            e.SectorTopoImageId,
            e.SectorTopoImageId.HasValue ? await service.GetImageUrlAsync(e.SectorTopoImageId.Value, ct) : null,
            e.SectorTopoImageOverlaySvgId.HasValue ? await service.GetImageUrlAsync(e.SectorTopoImageOverlaySvgId.Value, ct) : null
        );
    }
}