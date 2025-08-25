using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public record DeleteAreaRequest(Guid AreaId);

public class DeleteArea : Endpoint<DeleteAreaRequest, EmptyResponse>
{
    private readonly IEntityRepository<Area> _areaRepository;

    public DeleteArea(IEntityRepository<Area> areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public override void Configure()
    {
        Delete("/{AreaId}");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteAreaRequest req, CancellationToken ct)
    {
        var area = await _areaRepository.GetByIdAsync(req.AreaId, ct);
        
        if (area == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await _areaRepository.DeleteAsync(area, ct);
        await SendNoContentAsync(ct);
    }
} 