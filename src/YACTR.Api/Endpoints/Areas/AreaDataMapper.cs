using FastEndpoints;
using NetTopologySuite.Geometries;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Areas;

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