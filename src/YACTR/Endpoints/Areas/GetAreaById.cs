using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public record GetAreaByIdRequest(Guid AreaId);

public class GetAreaById : Endpoint<GetAreaByIdRequest, Area>
{
    private readonly IEntityRepository<Area> _areaRepository;

    public GetAreaById(IEntityRepository<Area> areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public override void Configure()
    {
        Get("/{AreaId}");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(GetAreaByIdRequest req, CancellationToken ct)
    {
        var area = await _areaRepository.GetByIdAsync(req.AreaId, ct);
        
        if (area == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(area, cancellation: ct);
    }
} 