using FastEndpoints;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Areas;

public record GetAreaByIdRequest(Guid AreaId);

public class GetAreaById : Endpoint<GetAreaByIdRequest, AreaResponse, AreaDataMapper>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Get("/{AreaId}");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(GetAreaByIdRequest req, CancellationToken ct)
    {
        var area = await AreaRepository.GetByIdAsync(req.AreaId, ct);

        if (area == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(Map.FromEntity(area), cancellation: ct);
    }
}