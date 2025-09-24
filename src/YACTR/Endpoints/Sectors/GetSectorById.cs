using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Sectors;

public record GetSectorByIdRequest(Guid SectorId);

public class GetSectorById : Endpoint<GetSectorByIdRequest, Sector>
{
    private readonly IEntityRepository<Sector> _sectorRepository;

    public GetSectorById(IEntityRepository<Sector> sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public override void Configure()
    {
        Get("/{SectorId}");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(GetSectorByIdRequest req, CancellationToken ct)
    {
        var sector = await _sectorRepository.GetByIdAsync(req.SectorId, ct);
        
        if (sector == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(sector, cancellation: ct);
    }
} 