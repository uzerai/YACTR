using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Sectors;

public class GetAllSectors : Endpoint<EmptyRequest, List<SectorResponse>, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<SectorsEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var sectors = await SectorRepository.GetAllAvailableAsync(ct);
        var response = sectors.Select(async (e) => await Map.FromEntityAsync(e, ct));

        await Task.WhenAll(response);
        await Send.OkAsync([.. response.Select(r => r.Result)], cancellation: ct);
    }
}