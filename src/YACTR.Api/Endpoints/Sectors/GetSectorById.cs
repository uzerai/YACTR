using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.QueryExtensions;

namespace YACTR.Api.Endpoints.Sectors;

public record GetSectorByIdRequest(Guid SectorId);

public class GetSectorById : Endpoint<GetSectorByIdRequest, SectorResponse, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Get("/{sector_id}");
        Group<SectorsEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSectorByIdRequest req, CancellationToken ct)
    {
        var sector = await SectorRepository
            .BuildReadonlyQuery()
            .WhereAvailable()
            .Where(e => e.Id == req.SectorId)
            .Include(e => e.Area)
            .Include("SectorImages")
            .FirstOrDefaultAsync(ct);

        if (sector == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(await Map.FromEntityAsync(sector, ct), cancellation: ct);
    }
}