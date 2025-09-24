namespace YACTR.Endpoints.Routes.RouteLikes.ViewModels;

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
