using FastEndpoints;
using NetTopologySuite.Geometries;
using YACTR.Data.Model.Climbing;

namespace YACTR.Endpoints.Areas;

/// <summary>
/// Request data for creating and updating the <see cref="Area"/> entity.
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

public record AreaResponse(
    Guid Id,
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary
);

public class AreaDataMapper : Mapper<AreaRequestData, AreaResponse, Area>
{
    public override Area ToEntity(AreaRequestData r) => new()
    {
        Name = r.Name,
        Description = r.Description,
        Location = r.Location,
        Boundary = r.Boundary,
    };

    public override AreaResponse FromEntity(Area e) => new(e.Id, e.Name, e.Description, e.Location, e.Boundary);

    public override Area UpdateEntity(AreaRequestData r, Area e)
    {
        e.Name = r.Name;
        e.Description = r.Description;
        e.Location = r.Location;
        e.Boundary = r.Boundary;

        return e;
    }
}