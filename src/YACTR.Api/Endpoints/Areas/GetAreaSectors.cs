using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Api.Endpoints.Sectors;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Areas;

public record GetAreaSectorsRequest(Guid AreaId);

public class GetAreaSectors : Endpoint<GetAreaSectorsRequest, IEnumerable<SectorResponse>, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Get("/{area_id}/sectors");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(GetAreaSectorsRequest req, CancellationToken ct)
    {
        var areaExists = await AreaRepository.BuildReadonlyQuery().AnyAsync(e => e.Id == req.AreaId, ct);
        if (!areaExists)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var sectors = await SectorRepository.AllAvailable()
            .Where(e => e.AreaId == req.AreaId)
            .ToListAsync(ct);

        await Send.OkAsync(
            await Task.WhenAll(sectors.Select(async e => await Map.FromEntityAsync(e, ct))),
            cancellation: ct);
    }
}