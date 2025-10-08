using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Areas;

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

        await SendCreatedAtAsync<GetAreaById>(createdArea.Id, Map.FromEntity(createdArea), FastEndpoints.Http.GET, cancellation: ct);
    }
}