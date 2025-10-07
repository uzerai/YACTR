using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Sectors;

public class GetAllSectors : Endpoint<EmptyRequest, List<Sector>>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var sectors = await SectorRepository.GetAllAsync(ct);
        await SendAsync([.. sectors], cancellation: ct);
    }
}