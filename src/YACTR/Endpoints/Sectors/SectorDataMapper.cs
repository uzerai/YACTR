using FastEndpoints;
using NetTopologySuite.Geometries;
using YACTR.Data.Model.Climbing;
using YACTR.DI.Service;

namespace YACTR.Endpoints.Sectors;

/// <summary>
/// Request data for creating and updating the <see cref="Sector"/> entity.
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
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid AreaId
);

public record SectorResponse(
    Guid Id,
    string Name,
    Polygon SectorArea,
    Point EntryPoint,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid AreaId,
    Guid? SectorImageId,
    string? SectorImageUrl
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
    };

    public override async Task<SectorResponse> FromEntityAsync(Sector e, CancellationToken ct)
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
            e.SectorImageId,
            e.SectorImageId.HasValue ? await service.GetImageUrlAsync(e.SectorImageId.Value, ct) : null
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
