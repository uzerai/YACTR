using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Sectors;

public class CreateSector : AuthenticatedEndpoint<SectorRequestData, Sector>
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
        var createdSector = await SectorRepository.CreateAsync(new()
        {
            Name = req.Name,
            SectorArea = req.SectorArea,
            EntryPoint = req.EntryPoint,
            AreaId = req.AreaId,
            RecommendedParkingLocation = req.RecommendedParkingLocation,
            ApproachPath = req.ApproachPath,
        }, ct);

        await SendCreatedAtAsync<GetSectorById>(createdSector.Id, createdSector, cancellation: ct);
    }
}