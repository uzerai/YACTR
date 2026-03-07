using FastEndpoints;
using FluentValidation;
using NetTopologySuite.Geometries;

namespace YACTR.Api.Endpoints.Areas;

/// <summary>
/// Request data for creating and updating the <see cref="YACTR.Domain.Model.Climbing.Area"/> entity.
/// </summary>
/// <param name="Name">The name of the area.</param>
/// <param name="Description">The description of the area.</param>
/// <param name="Location">The location of the area.</param>
/// <param name="Boundary">The boundary of the area.</param>
public record AreaRequestData(
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary
);

public class AreaRequestDataValidator : Validator<AreaRequestData>
{
    public AreaRequestDataValidator()
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