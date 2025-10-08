using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Sectors;

public record GetSectorByIdRequest(Guid SectorId);

public class GetSectorById : Endpoint<GetSectorByIdRequest, SectorResponse, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Get("/{SectorId}");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(GetSectorByIdRequest req, CancellationToken ct)
    {
        var sector = await SectorRepository.GetByIdAsync(req.SectorId, ct);

        if (sector == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(await Map.FromEntityAsync(sector, ct), cancellation: ct);
    }
}