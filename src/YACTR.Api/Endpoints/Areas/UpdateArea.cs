using FastEndpoints;
using FluentValidation;
using NetTopologySuite.Geometries;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Areas;

public record UpdateAreaBody(
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary
);

public class UpdateAreaRequest
{
    public Guid AreaId { get; set; }

    [FromBody]
    public required UpdateAreaBody Data { get; set; }
}

public class UpdateAreaRequestValidator : Validator<UpdateAreaRequest>
{
    public UpdateAreaRequestValidator()
    {
        RuleFor(x => x.Data.Name)
            .NotEmpty()
            .MaximumLength(255)
            .MinimumLength(2);
        RuleFor(x => x.Data.Description)
            .MaximumLength(1000)
            .When(x => x.Data.Description is not null);
        RuleFor(x => x.Data.Location)
            .NotEmpty()
            .Must(x => x.IsEmpty == false);
        RuleFor(x => x.Data.Boundary)
            .NotEmpty()
            .Must(x => x.IsEmpty == false);
    }
}

public class UpdateArea : AuthenticatedEndpoint<UpdateAreaRequest, EmptyResponse>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Put("/{area_id}");
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

        area.Name = req.Data.Name;
        area.Description = req.Data.Description;
        area.Location = req.Data.Location;
        area.Boundary = req.Data.Boundary;

        await AreaRepository.UpdateAsync(area, ct);

        await Send.NoContentAsync(ct);
    }
}