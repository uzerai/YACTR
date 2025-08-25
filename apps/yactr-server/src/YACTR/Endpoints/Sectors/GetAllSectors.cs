using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public class GetAllSectors : Endpoint<EmptyRequest, List<Sector>>
{
    private readonly IEntityRepository<Sector> _sectorRepository;

    public GetAllSectors(IEntityRepository<Sector> sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var sectors = await _sectorRepository.GetAllAsync(ct);
        await SendAsync([.. sectors], cancellation: ct);
    }
} 