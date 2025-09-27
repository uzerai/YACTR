using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Sectors;

public class UpdateSectorRequest
{
    public Guid SectorId { get; set; }

    [FromBody]
    public required SectorRequestData Data { get; set; }

}

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

        existingSector.Name = req.Data.Name;
        existingSector.SectorArea = req.Data.SectorArea;
        existingSector.EntryPoint = req.Data.EntryPoint;
        existingSector.RecommendedParkingLocation = req.Data.RecommendedParkingLocation;
        existingSector.ApproachPath = req.Data.ApproachPath;
        existingSector.AreaId = req.Data.AreaId;

        await _sectorRepository.UpdateAsync(existingSector, ct);

        await SendNoContentAsync(ct);
    }
}