using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.QueryExtensions;

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

        Map.UpdateEntity(req.Data, existingSector);

        await SectorRepository.SaveAsync(ct);

        await Send.NoContentAsync(ct);
    }
}