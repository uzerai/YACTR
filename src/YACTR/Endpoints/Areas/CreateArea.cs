using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Areas;

public class CreateArea : AuthenticatedEndpoint<AreaRequestData, Area>
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
        var createdArea = await AreaRepository.CreateAsync(new()
        {
            Name = req.Name,
            Description = req.Description,
            Location = req.Location,
            Boundary = req.Boundary,
        }, ct);

        await SendCreatedAtAsync<GetAreaById>(createdArea.Id, createdArea, cancellation: ct);
    }
}