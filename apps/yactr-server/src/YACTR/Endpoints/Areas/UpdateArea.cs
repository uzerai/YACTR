using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Areas;

public class UpdateAreaRequest
{
    public Guid AreaId { get; set; }

    [FromBody]
    public required AreaRequestData Data { get; set; }
}

public class UpdateArea : AuthenticatedEndpoint<UpdateAreaRequest, EmptyResponse, AreaDataMapper>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Put("/{AreaId}");
        Group<AreasEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.AreasWrite)));
    }

    public override async Task HandleAsync(UpdateAreaRequest req, CancellationToken ct)
    {
        var area = await AreaRepository.GetByIdAsync(req.AreaId, ct);

        if (area == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Map.UpdateEntity(req.Data, area);

        await AreaRepository.UpdateAsync(area, ct);

        await Send.NoContentAsync(ct);
    }
}