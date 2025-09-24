namespace YACTR.Endpoints.Routes.RouteRatings.ViewModels;

/// <summary>
/// Response data for RouteRating entities.
/// </summary>
/// <param name="Id">The ID of the route rating.</param>
/// <param name="UserId">The ID of the user who rated the route.</param>
/// <param name="RouteId">The ID of the route that was rated.</param>
/// <param name="Rating">The rating value (1-5).</param>
public record RouteRatingResponse(
    Guid Id,
    Guid UserId,
    Guid RouteId,
    int Rating
);

/// <summary>
/// Request data for creating and updating RouteRating entities.
/// </summary>
/// <param name="Rating">The rating value (1-5).</param>
public record RouteRatingRequest(
    int Rating
);