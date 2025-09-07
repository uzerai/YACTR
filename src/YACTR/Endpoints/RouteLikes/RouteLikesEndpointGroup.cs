using FastEndpoints;
using YACTR.Data.Model.Climbing.Rating;

namespace YACTR.Endpoints;

/// <summary>
/// Response data for RouteLike entities.
/// </summary>
/// <param name="Id">The ID of the route like.</param>
/// <param name="UserId">The ID of the user who liked the route.</param>
/// <param name="RouteId">The ID of the route that was liked.</param>
public record RouteLikeResponse(
    Guid Id,
    Guid UserId,
    Guid RouteId
);

public class RouteLikesEndpointGroup : Group
{
    public RouteLikesEndpointGroup()
    {
        Configure("routes", ep => {});
    }
}
