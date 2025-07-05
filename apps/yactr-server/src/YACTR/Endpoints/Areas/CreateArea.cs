using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public class CreateArea : Endpoint<AreaRequestData, Area>
{
    private readonly IEntityRepository<Area> _areaRepository;

    public CreateArea(IEntityRepository<Area> areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public override void Configure()
    {
        Post("/");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(AreaRequestData req, CancellationToken ct)
    {
        var createdArea = await _areaRepository.CreateAsync(new()
        {
            Name = req.Name,
            Description = req.Description,
            Location = req.Location,
            Boundary = req.Boundary
        }, ct);

        await SendCreatedAtAsync<GetAreaById>(createdArea.Id, createdArea, cancellation: ct);
    }
} 