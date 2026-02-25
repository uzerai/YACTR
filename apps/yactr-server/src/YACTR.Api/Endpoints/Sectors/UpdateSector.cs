using FastEndpoints;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Sectors;

public class UpdateSectorRequest
{
    public Guid SectorId { get; set; }

    [FromBody]
    public required SectorRequestData Data { get; set; }

}

public class UpdateSector : AuthenticatedEndpoint<UpdateSectorRequest, EmptyResponse, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Put("/{SectorId}");
        Group<SectorsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.SectorsWrite)));
    }

    public override async Task HandleAsync(UpdateSectorRequest req, CancellationToken ct)
    {
        var existingSector = await SectorRepository.GetByIdAsync(req.SectorId, ct);
        if (existingSector == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Map.UpdateEntity(req.Data, existingSector);

        await SectorRepository.UpdateAsync(existingSector, ct);

        await Send.NoContentAsync(ct);
    }
}