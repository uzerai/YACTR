using FastEndpoints;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Areas;

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
        await Send.OkAsync([.. areas.Select(Map.FromEntity)], cancellation: ct);
    }
}