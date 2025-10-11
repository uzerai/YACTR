using FastEndpoints;
using NodaTime;
using YACTR.Data.Model;
using YACTR.Data.Model.Climbing;
using YACTR.DI.Service;
using YACTR.Endpoints.Images;
using YACTR.Endpoints.Sectors;
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
    Guid? TopoImageId = null,
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
    string? Grade,
    Instant? FirstAscentDate,
    string? FirstAscentClimberName,
    string? BolterName,
    Guid? TopoImageId,
    string? TopoImageUrl,
    string? TopoImageOverlaySVG
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
            e.TopoImage != null ? await service.GetImageUrlAsync(e.TopoImage.Key, e.TopoImage.Bucket, ct) : null,
            e.TopoImageOverlaySVG
        );
    }
}