using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Sectors;

public record UpdateSectorRequest(Guid SectorId);

public class UpdateSector : Endpoint<UpdateSectorRequest, EmptyResponse>
{
    private readonly IEntityRepository<Sector> _sectorRepository;

    public UpdateSector(IEntityRepository<Sector> sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public override void Configure()
    {
        Put("/{SectorId}");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateSectorRequest req, CancellationToken ct)
    {
        var existingSector = await _sectorRepository.GetByIdAsync(req.SectorId, ct);
        if (existingSector == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Note: This would need to be implemented with proper request body handling
        // The original controller had incomplete update logic
        await _sectorRepository.UpdateAsync(existingSector, ct);
        
        await SendNoContentAsync(ct);
    }
} 