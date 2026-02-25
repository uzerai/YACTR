using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Areas;

public class CreateArea : AuthenticatedEndpoint<AreaRequestData, AreaResponse, AreaDataMapper>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<AreasEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.AreasWrite)));
    }

    public override async Task HandleAsync(AreaRequestData req, CancellationToken ct)
    {
        var createdArea = await AreaRepository.CreateAsync(Map.ToEntity(req), ct);

        await Send.CreatedAtAsync<GetAreaById>(createdArea.Id, Map.FromEntity(createdArea), FastEndpoints.Http.GET, cancellation: ct);
    }
}