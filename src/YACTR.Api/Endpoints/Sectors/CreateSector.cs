using NetTopologySuite.Geometries;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Sectors;

public record CreateSectorImageRequest(
    Guid ImageId,
    int Order
);

public record CreateSectorRequest(
    string Name,
    Polygon SectorArea,
    Point? EntryPoint,
    Guid AreaId,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    IEnumerable<CreateSectorImageRequest>? SectorImages,
    Guid? PrimarySectorImageId
);

public record CreatedSectorResponse(
    Guid Id,
    string Name,
    Polygon SectorArea,
    Point? EntryPoint,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid? PrimarySectorImageId,
    IEnumerable<Guid> SectorImageIds,
    Guid AreaId
);

public class CreateSector : AuthenticatedEndpoint<CreateSectorRequest, CreatedSectorResponse>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<SectorsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.SectorsWrite)));
    }

    public override async Task HandleAsync(CreateSectorRequest req, CancellationToken ct)
    {
        var sectorImages = req.SectorImages?
            .Select(s => new SectorImage
            {
                ImageId = s.ImageId,
                Order = s.Order
            })
            .ToList() ?? [];

        var primarySectorImageId = req.PrimarySectorImageId
            ?? sectorImages
                .OrderBy(si => si.Order)
                .Select(si => (Guid?)si.ImageId)
                .FirstOrDefault();

        var sector = new Sector
        {
            Name = req.Name,
            SectorArea = req.SectorArea,
            EntryPoint = req.EntryPoint,
            RecommendedParkingLocation = req.RecommendedParkingLocation,
            ApproachPath = req.ApproachPath,
            AreaId = req.AreaId,
            PrimarySectorImageId = primarySectorImageId,
            SectorImages = sectorImages,
        };

        var createdSector = await SectorRepository.CreateAsync(sector, ct);

        await Send.CreatedAtAsync<GetSectorById>(createdSector.Id,
            new CreatedSectorResponse(createdSector.Id,
                createdSector.Name,
                createdSector.SectorArea,
                createdSector.EntryPoint,
                createdSector.RecommendedParkingLocation,
                createdSector.ApproachPath,
                createdSector.PrimarySectorImageId,
                createdSector.SectorImages.Select(si => si.ImageId),
                createdSector.AreaId)
            , cancellation: ct);
    }
}