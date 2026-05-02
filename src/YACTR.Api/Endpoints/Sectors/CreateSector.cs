using NetTopologySuite.Geometries;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Sectors;

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

public class CreateSector : AuthenticatedEndpoint<SectorRequestData, CreatedSectorResponse, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<SectorsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.SectorsWrite)));
    }

    public override async Task HandleAsync(SectorRequestData req, CancellationToken ct)
    {
        var sector = Map.ToEntity(req);
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