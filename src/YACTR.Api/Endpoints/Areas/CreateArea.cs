using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Areas;

public record CreateAreaRequest(
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary
);

public class CreateAreaRequestValidator : Validator<CreateAreaRequest>
{
    public CreateAreaRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .MinimumLength(2);
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
        RuleFor(x => x.Location)
            .NotEmpty()
            .Must(x => x.IsEmpty == false);
        RuleFor(x => x.Boundary)
            .NotEmpty()
            .Must(x => x.IsEmpty == false);
    }
}

public record CreateAreaResponse(
    Guid Id,
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary,
    Instant CreatedAt,
    Instant UpdatedAt
);

public class CreateArea : AuthenticatedEndpoint<CreateAreaRequest, CreateAreaResponse>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }
    public required IRepository<CountryData> CountryDataRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<AreasEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.AreasWrite)));
    }

    public override async Task HandleAsync(CreateAreaRequest req, CancellationToken ct)
    {
        // Closest country to the area's location.
        var nearestCountry = await CountryDataRepository.BuildTrackedQuery()
            .Where(e => e.Geometry.IsWithinDistance(req.Location, 10000))
            .OrderBy(e => e.Geometry.Distance(req.Location))
            .FirstOrDefaultAsync(ct);

        if (nearestCountry == null)
        {
            AddError(r => r.Location, "Provided location is too far from any country");

            // The area is > 10km from any country, so we don't create it.
            await Send.ErrorsAsync(412, cancellation: ct);
            return;
        }

        var newEntity = new Area
        {
            Name = req.Name,
            Description = req.Description,
            Location = req.Location,
            Boundary = req.Boundary,
            CountryId = nearestCountry.Id,
            Country = nearestCountry,
        };

        var createdArea = await AreaRepository.CreateAsync(newEntity, ct);

        await Send.CreatedAtAsync<GetAreaById>(createdArea.Id,
            new CreateAreaResponse(
                Id: createdArea.Id,
                Name: createdArea.Name,
                Description: createdArea.Description,
                Location: createdArea.Location,
                Boundary: createdArea.Boundary,
                CreatedAt: createdArea.CreatedAt,
                UpdatedAt: createdArea.UpdatedAt
            ), cancellation: ct);
    }
}