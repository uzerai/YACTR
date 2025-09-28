using FastEndpoints;
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
    string? Description,
    string? Grade,
    string? FirstAscentClimberName,
    string? BolterName);

public class RoutesEndpointGroup : Group
{
    public RoutesEndpointGroup()
    {
        Configure("routes", ep => { });
    }
}