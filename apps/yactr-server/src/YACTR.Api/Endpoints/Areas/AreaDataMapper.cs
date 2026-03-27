using FastEndpoints;
using NetTopologySuite.Geometries;
using NodaTime;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Areas;

public record AreaResponse(
    Guid Id,
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary,
    Instant CreatedAt,
    Instant UpdatedAt
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

    public override AreaResponse FromEntity(Area e) => new(e.Id, e.Name, e.Description, e.Location, e.Boundary, e.CreatedAt, e.UpdatedAt);

    public override Task<AreaResponse> FromEntityAsync(Area e, CancellationToken ct = default)
        => Task.FromResult(FromEntity(e));

    public override Area UpdateEntity(AreaRequestData r, Area e)
    {
        e.Name = r.Name;
        e.Description = r.Description;
        e.Location = r.Location;
        e.Boundary = r.Boundary;

        return e;
    }
}