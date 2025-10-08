using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Areas;

public class GetAllAreas : Endpoint<EmptyRequest, List<AreaResponse>, AreaDataMapper>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<AreasEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var areas = await AreaRepository.GetAllAvailableAsync(ct);
        await SendAsync([.. areas.Select(Map.FromEntity)], cancellation: ct);
    }
}