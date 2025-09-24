using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public class UpdateAreaRequest {
    public Guid AreaId { get; set; }

    [FromBody]
    public required AreaRequestData Area { get; set; }
}

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

        area.Name = req.Area.Name;
        area.Description = req.Area.Description;
        area.Location = req.Area.Location;
        area.Boundary = req.Area.Boundary;

        await _areaRepository.UpdateAsync(area, ct);
        
        await SendNoContentAsync(ct);
    }
} 