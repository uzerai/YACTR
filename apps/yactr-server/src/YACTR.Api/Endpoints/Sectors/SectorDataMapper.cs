using FastEndpoints;
using NetTopologySuite.Geometries;
using NodaTime;
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
    Point? EntryPoint,
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
    Point? EntryPoint,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid AreaId,
    string AreaName,
    Guid? PrimarySectorImageId,
    string? PrimarySectorImageUrl,
    IEnumerable<SectorImageResponseData> SectorImages,
    Instant CreatedAt,
    Instant UpdatedAt
);

public class SectorDataMapper : Mapper<SectorRequestData, SectorResponse, Sector>
{
    public override Sector ToEntity(SectorRequestData r)
    {
        var sectorImages = r.SectorImages?
            .Select(s => new SectorImage
            {
                ImageId = s.ImageId,
                Order = s.Order
            })
            .ToList() ?? [];

        var primarySectorImageId = r.PrimarySectorImageId
            ?? sectorImages
                .OrderBy(si => si.Order)
                .Select(si => (Guid?)si.ImageId)
                .FirstOrDefault();

        return new()
        {
            Name = r.Name,
            SectorArea = r.SectorArea,
            EntryPoint = r.EntryPoint,
            RecommendedParkingLocation = r.RecommendedParkingLocation,
            ApproachPath = r.ApproachPath,
            AreaId = r.AreaId,
            PrimarySectorImageId = primarySectorImageId,
            SectorImages = sectorImages,
        };
    }

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
            e.Area.Name,
            e.PrimarySectorImageId,
            e.PrimarySectorImageId.HasValue ? await service.GetImageUrlAsync(e.PrimarySectorImageId.Value, ct) : null,
            await Task.WhenAll(e.SectorImages.Select(async sI => new SectorImageResponseData(sI.ImageId, sI.Order, await service.GetImageUrlAsync(sI.ImageId, ct)))),
            e.CreatedAt,
            e.UpdatedAt
        );
    }

    public override Sector UpdateEntity(SectorRequestData r, Sector e)
    {
        e.Name = r.Name;
        e.SectorArea = r.SectorArea;
        e.EntryPoint = r.EntryPoint;
        e.RecommendedParkingLocation = r.RecommendedParkingLocation;
        e.ApproachPath = r.ApproachPath;
        e.PrimarySectorImageId = r.PrimarySectorImageId;

        // Update the sector images collection in-place since EF is fucking stupid as fuck.
        if (r.SectorImages != null)
        {
            var requested = r.SectorImages.ToDictionary(x => x.ImageId, x => x.Order);

            // Update existing in case order has updated.
            foreach (var existing in e.SectorImages.ToList())
            {
                if (requested.TryGetValue(existing.ImageId, out var requestedOrder))
                {
                    existing.Order = requestedOrder;
                }
                else
                {
                    e.SectorImages.Remove(existing);
                }
            }

            // Add missing relations.
            foreach (var (imageId, order) in requested)
            {
                if (e.SectorImages.All(si => si.ImageId != imageId))
                {
                    e.SectorImages.Add(new SectorImage
                    {
                        SectorId = e.Id,
                        ImageId = imageId,
                        Order = order
                    });
                }
            }

            // If no primary image is explicitly requested, default to the first image (lowest order)
            // from the provided SectorImages list.
            if (r.PrimarySectorImageId == null)
            {
                e.PrimarySectorImageId = r.SectorImages
                    .OrderBy(si => si.Order)
                    .Select(si => (Guid?)si.ImageId)
                    .FirstOrDefault();
            }
        }

        return e;
    }
}