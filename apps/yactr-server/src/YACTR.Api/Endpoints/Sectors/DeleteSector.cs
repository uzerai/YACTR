using FastEndpoints;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Sectors;

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
            await Send.NotFoundAsync(ct);
            return;
        }

        await SectorRepository.DeleteAsync(sector, ct);
        await Send.NoContentAsync(ct);
    }
}