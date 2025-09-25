using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Areas;

public class GetAllAreas : Endpoint<EmptyRequest, List<Area>>
{
    private readonly IEntityRepository<Area> _areaRepository;

    public GetAllAreas(IEntityRepository<Area> areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var areas = await _areaRepository.GetAllAvailableAsync(ct);
        await SendAsync([.. areas], cancellation: ct);
    }
}