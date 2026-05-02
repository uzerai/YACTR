using FastEndpoints;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Areas;

public record DeleteAreaRequest(Guid AreaId);

public class DeleteArea : AuthenticatedEndpoint<DeleteAreaRequest, EmptyResponse>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Delete("/{area_id}");
        Group<AreasEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.AreasWrite)));
    }

    public override async Task HandleAsync(DeleteAreaRequest req, CancellationToken ct)
    {
        var area = await AreaRepository.GetByIdAsync(req.AreaId, ct);

        if (area == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await AreaRepository.DeleteAsync(area, ct);
        await Send.NoContentAsync(ct);
    }
}