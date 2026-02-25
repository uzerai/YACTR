using FastEndpoints;
using NetTopologySuite.Geometries;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Endpoints.Sectors;

/// <summary>
/// Request data for creating and updating the <see cref="YACTR.Domain.Model.Climbing.Sector"/> entity.
/// </summary>
/// <param name="Name">The name of the sector.</param>
/// <param name="SectorArea">The area of the sector.</param>
/// <param name="EntryPoint">The entry point of the sector.</param>
/// <param name="RecommendedParkingLocation">The recommended parking location for the sector.</param>
/// <param name="ApproachPath">The approach path to the sector.</param>
/// <param name="AreaId">The ID of the area this sector belongs to.</param>
public record SectorRequestData(
    string Name,
    Polygon SectorArea,
    Point EntryPoint,
    Guid AreaId,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    IEnumerable<SectorImageRequestData>? SectorImages,
    Guid? PrimarySectorImageId
);

public record SectorImageRequestData(
    Guid ImageId,
    int Order
);

public record SectorImageResponseData(
    Guid ImageId,
    int Order,
    string? ImageUrl
);

public record SectorResponse(
    Guid Id,
    string Name,
    Polygon SectorArea,
    Point EntryPoint,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid AreaId,
    Guid? PrimarySectorImageId,
    string? PrimarySectorImageUrl,
    IEnumerable<SectorImageResponseData> SectorImages
);

public class SectorDataMapper : Mapper<SectorRequestData, SectorResponse, Sector>
{
    public override Sector ToEntity(SectorRequestData r) => new()
    {
        Name = r.Name,
        SectorArea = r.SectorArea,
        EntryPoint = r.EntryPoint,
        RecommendedParkingLocation = r.RecommendedParkingLocation,
        ApproachPath = r.ApproachPath,
        AreaId = r.AreaId,
        PrimarySectorImageId = r.PrimarySectorImageId ?? r.SectorImages?.FirstOrDefault()?.ImageId ?? null,
        SectorImages = r.SectorImages?.Select(s => new SectorImage()
        {
            ImageId = s.ImageId,
            Order = s.Order
        }).ToList() ?? [],
    };

    public override async Task<SectorResponse> FromEntityAsync(Sector e, CancellationToken ct = default)
    {
        // The creation of a new scope enables multi-threaded mapping (ie; we concurrently map all entities asynchronously)
        using var mappingScope = CreateScope();
        IImageStorageService service = mappingScope.Resolve<IImageStorageService>();

        return new(
            e.Id,
            e.Name,
            e.SectorArea,
            e.EntryPoint,
            e.RecommendedParkingLocation,
            e.ApproachPath,
            e.AreaId,
            e.PrimarySectorImageId,
            e.PrimarySectorImageId.HasValue ? await service.GetImageUrlAsync(e.PrimarySectorImageId.Value, ct) : null,
            await Task.WhenAll(e.SectorImages.Select(async sI => new SectorImageResponseData(sI.ImageId, sI.Order, await service.GetImageUrlAsync(sI.ImageId, ct))))
        );
    }

    public override Sector UpdateEntity(SectorRequestData r, Sector e)
    {
        e.Name = r.Name;
        e.SectorArea = r.SectorArea;
        e.EntryPoint = r.EntryPoint;
        e.RecommendedParkingLocation = r.RecommendedParkingLocation;
        e.ApproachPath = r.ApproachPath;

        return e;
    }
}