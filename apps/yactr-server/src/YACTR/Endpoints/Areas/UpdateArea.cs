using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public record UpdateAreaRequest(Guid AreaId);

public class UpdateArea : Endpoint<UpdateAreaRequest, EmptyResponse>
{
    private readonly IEntityRepository<Area> _areaRepository;

    public UpdateArea(IEntityRepository<Area> areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public override void Configure()
    {
        Put("/{AreaId}");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateAreaRequest req, CancellationToken ct)
    {
        var area = await _areaRepository.GetByIdAsync(req.AreaId, ct);

        if (area == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Note: In the original controller, the update logic was incomplete
        // This would need to be implemented based on the actual requirements
        await _areaRepository.UpdateAsync(area, ct);
        
        await SendNoContentAsync(ct);
    }
} 