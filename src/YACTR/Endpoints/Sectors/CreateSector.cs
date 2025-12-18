using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Sectors;

public class CreateSector : AuthenticatedEndpoint<SectorRequestData, SectorResponse, SectorDataMapper>
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

        await Send.CreatedAtAsync<GetSectorById>(createdSector.Id, await Map.FromEntityAsync(createdSector, ct), cancellation: ct);
    }
}