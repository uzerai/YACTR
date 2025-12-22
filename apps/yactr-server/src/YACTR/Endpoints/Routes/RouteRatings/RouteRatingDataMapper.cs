using FastEndpoints;
using YACTR.Data.Model.Climbing.Rating;

namespace YACTR.Endpoints.Routes.RouteRatings;

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
public record RouteRatingRequestData(
    int Rating
);

public class RouteRatingDataMapper : Mapper<RouteRatingRequestData, RouteRatingResponse, RouteRating>
{
    public override async Task<RouteRatingResponse> FromEntityAsync(RouteRating e, CancellationToken ct = default)
    {
        return new RouteRatingResponse(e.Id, e.UserId, e.RouteId, e.Rating);
    }

    public override async Task<RouteRating> UpdateEntityAsync(RouteRatingRequestData r, RouteRating e, CancellationToken ct = default)
    {
        e.Rating = r.Rating;

        return e;
    }
}