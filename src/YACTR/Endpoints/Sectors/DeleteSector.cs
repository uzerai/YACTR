using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Sectors;

public record DeleteSectorRequest(Guid SectorId);

public class DeleteSector : AuthenticatedEndpoint<DeleteSectorRequest, EmptyResponse>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Delete("/{SectorId}");
        Group<SectorsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.SectorsWrite)));
    }

    public override async Task HandleAsync(DeleteSectorRequest req, CancellationToken ct)
    {
        var sector = await SectorRepository.GetByIdAsync(req.SectorId, ct);

        if (sector == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SectorRepository.DeleteAsync(sector, ct);
        await SendNoContentAsync(ct);
    }
}