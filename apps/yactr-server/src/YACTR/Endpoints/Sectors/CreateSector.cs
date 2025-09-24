using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Sectors;

public class CreateSector : Endpoint<SectorRequestData, Sector>
{
    private readonly IEntityRepository<Sector> _sectorRepository;

    public CreateSector(IEntityRepository<Sector> sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public override void Configure()
    {
        Post("/");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(SectorRequestData req, CancellationToken ct)
    {
        var createdSector = await _sectorRepository.CreateAsync(new()
        {
            Name = req.Name,
            SectorArea = req.SectorArea,
            EntryPoint = req.EntryPoint,
            AreaId = req.AreaId,
            RecommendedParkingLocation = req.RecommendedParkingLocation,
            ApproachPath = req.ApproachPath,
        }, ct);

        await SendCreatedAtAsync<GetSectorById>(createdSector.Id, createdSector, cancellation: ct);
    }
} 