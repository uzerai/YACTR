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
        var newEntity = Map.ToEntity(req);

        // Closest country to the area's location.
        var nearestCountry = await CountryDataRepository.BuildReadonlyQuery()
            .Where(e => e.Geometry.IsWithinDistance(newEntity.Location, 10000))
            .OrderBy(e => e.Geometry.Distance(newEntity.Location))
            .FirstOrDefaultAsync(ct);

        if (nearestCountry == null)
        {
            AddError(r => r.Location, "Provided location is too far from any country");
            
            // The area is > 10km from any country, so we don't create it.
            await Send.ErrorsAsync(412, cancellation: ct);
            return;
        }

        newEntity.CountryId = nearestCountry.Id;

        var createdArea = await AreaRepository.CreateAsync(newEntity, ct);

        await Send.CreatedAtAsync<GetAreaById>(createdArea.Id, Map.FromEntity(createdArea), cancellation: ct);
    }
}