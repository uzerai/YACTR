using FastEndpoints;
using NetTopologySuite.Geometries;

namespace YACTR.Endpoints;

/// <summary>
/// Request data for creating and updating the <see cref="Area"/> entity.
/// </summary>
/// <param name="Name">The name of the area.</param>
/// <param name="Description">The description of the area.</param>
/// <param name="Location">The location of the area.</param>
/// <param name="Boundary">The boundary of the area.</param>
public record AreaRequestData(
    string Name,
    string Description,
    Point Location,
    MultiPolygon Boundary
);

public class AreasEndpointGroup : Group
{
    public AreasEndpointGroup()
    {
        Configure("areas", ep => {});
    }
} 