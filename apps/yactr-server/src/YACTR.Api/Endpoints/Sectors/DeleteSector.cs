using FastEndpoints;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Sectors;

public class DeleteSectorRequest
{
    [BindFrom("sector_id")]
    public required Guid SectorId { get; set; }
}

public class DeleteSector : AuthenticatedEndpoint<DeleteSectorRequest, EmptyResponse>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Delete("/{sector_id}");
        Group<SectorsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.SectorsWrite)));
    }

    public override async Task HandleAsync(DeleteSectorRequest req, CancellationToken ct)
    {
        var sector = await SectorRepository.GetByIdAsync(req.SectorId, ct);

        if (sector == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await SectorRepository.DeleteAsync(sector, ct);
        await Send.NoContentAsync(ct);
    }
}