using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.QueryExtensions;

namespace YACTR.Api.Endpoints.Sectors;

public record UpdateSectorImageRequest(
    Guid ImageId,
    int Order
);

public record UpdateSectorData(
    string Name,
    Polygon SectorArea,
    Point? EntryPoint,
    Guid AreaId,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    IEnumerable<UpdateSectorImageRequest>? SectorImages,
    Guid? PrimarySectorImageId
);

public class UpdateSectorRequest
{
    [BindFrom("sector_id")]
    public required Guid SectorId { get; set; }

    [FromBody]
    public required UpdateSectorData Data { get; set; }

}

public class UpdateSector : AuthenticatedEndpoint<UpdateSectorRequest, EmptyResponse>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Put("/{sector_id}");
        Group<SectorsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.SectorsWrite)));
    }

    public override async Task HandleAsync(UpdateSectorRequest req, CancellationToken ct)
    {
        // Load with tracking so the join entities (`sector_images`) can be updated in-place
        // instead of EF attempting to re-insert duplicate pivot rows.
        var existingSector = await SectorRepository.BuildTrackedQuery()
            .WhereAvailable()
            .Where(e => e.Id == req.SectorId)
            .Include("SectorImages")
            .FirstOrDefaultAsync(ct);
        if (existingSector == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        existingSector.Name = req.Data.Name;
        existingSector.SectorArea = req.Data.SectorArea;
        existingSector.EntryPoint = req.Data.EntryPoint;
        existingSector.RecommendedParkingLocation = req.Data.RecommendedParkingLocation;
        existingSector.ApproachPath = req.Data.ApproachPath;
        existingSector.PrimarySectorImageId = req.Data.PrimarySectorImageId;

        if (req.Data.SectorImages != null)
        {
            var requested = req.Data.SectorImages.ToDictionary(x => x.ImageId, x => x.Order);

            foreach (var existing in existingSector.SectorImages.ToList())
            {
                if (requested.TryGetValue(existing.ImageId, out var requestedOrder))
                {
                    existing.Order = requestedOrder;
                }
                else
                {
                    existingSector.SectorImages.Remove(existing);
                }
            }

            foreach (var (imageId, order) in requested)
            {
                if (existingSector.SectorImages.All(si => si.ImageId != imageId))
                {
                    existingSector.SectorImages.Add(new SectorImage
                    {
                        SectorId = existingSector.Id,
                        ImageId = imageId,
                        Order = order
                    });
                }
            }

            if (req.Data.PrimarySectorImageId == null)
            {
                existingSector.PrimarySectorImageId = req.Data.SectorImages
                    .OrderBy(si => si.Order)
                    .Select(si => (Guid?)si.ImageId)
                    .FirstOrDefault();
            }
        }

        await SectorRepository.SaveAsync(ct);

        await Send.NoContentAsync(ct);
    }
}