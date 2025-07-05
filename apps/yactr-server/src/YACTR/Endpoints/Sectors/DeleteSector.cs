using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public record DeleteSectorRequest(Guid SectorId);

public class DeleteSector : Endpoint<DeleteSectorRequest, EmptyResponse>
{
    private readonly IEntityRepository<Sector> _sectorRepository;

    public DeleteSector(IEntityRepository<Sector> sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public override void Configure()
    {
        Delete("/{SectorId}");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteSectorRequest req, CancellationToken ct)
    {
        var sector = await _sectorRepository.GetByIdAsync(req.SectorId, ct);
        
        if (sector == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await _sectorRepository.DeleteAsync(sector, ct);
        await SendNoContentAsync(ct);
    }
} 