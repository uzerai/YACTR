using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Areas;

public class CreateArea : AuthenticatedEndpoint<AreaRequestData, AreaResponse, AreaDataMapper>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }
    public required IRepository<CountryData> CountryDataRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<AreasEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.AreasWrite)));
    }

    public override async Task HandleAsync(AreaRequestData req, CancellationToken ct)
    {
        var possibleCountry = await CountryDataRepository.BuildReadonlyQuery()
            .FirstOrDefaultAsync(e => e.Geometry.Contains(req.Location), ct);

        if (possibleCountry == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var newEntity = Map.ToEntity(req);

        newEntity.CountryId = possibleCountry.Id;

        var createdArea = await AreaRepository.CreateAsync(newEntity, ct);

        await Send.CreatedAtAsync<GetAreaById>(createdArea.Id, Map.FromEntity(createdArea), cancellation: ct);
    }
}