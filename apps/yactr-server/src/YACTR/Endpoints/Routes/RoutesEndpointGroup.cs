using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Climbing;

namespace YACTR.Endpoints.Routes;

public record RoutePitchRequestData(
    string Name,
    ClimbingType Type,
    string? Description,
    string? Grade);

/// <summary>
/// Request data for creating and updating the <see cref="Route"/> entity.
/// </summary>
/// <param name="SectorId">The ID of the sector.</param>
/// <param name="Pitches">The pitches of the route.</param>
/// <param name="Name">The name of the route.</param>
public record RouteRequestData(
    Guid SectorId,
    RoutePitchRequestData[] Pitches,
    string Name,
    ClimbingType Type,
    Guid? TopoImageId = null,
    string? Description = null,
    string? Grade = null,
    string? FirstAscentClimberName = null,
    Instant? FirstAscentDate = null,
    string? BolterName = null);

public class RoutesEndpointGroup : Group
{
    public RoutesEndpointGroup()
    {
        Configure("routes", ep => { });
    }
}