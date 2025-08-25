using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Model.Location;

namespace YACTR.Endpoints;

/// <summary>
/// Request data for creating and updating Ascent entities.
/// </summary>
/// <param name="RouteId">The ID of the route that was ascended.</param>
/// <param name="Type">The type of ascent (Tick, Onsight, Flash, etc.).</param>
/// <param name="CompletedAt">When the ascent was completed.</param>
public record RouteAscentRequestData(
    Guid RouteId,
    AscentType Type,
    Instant CompletedAt
);

public record AscentResponse(
  Guid Id,
  Guid UserId,
  AscentType Type,
  Instant CompletedAt,
  Data.Model.Location.Route? Route
);

public class AscentsEndpointGroup : Group
{
    public AscentsEndpointGroup()
    {
        Configure("ascents", ep => {});
    }
} 