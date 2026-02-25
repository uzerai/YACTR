using FastEndpoints;
using YACTR.Domain.Model.Climbing.Rating;

namespace YACTR.Api.Endpoints.Routes.RouteLikes;

public record RouteLikeRequest(Guid RouteId);

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

public class RouteLikeDataMapper : Mapper<RouteLikeRequest, RouteLikeResponse, RouteLike>
{
    public override async Task<RouteLikeResponse> FromEntityAsync(RouteLike e, CancellationToken ct)
    {
        return new RouteLikeResponse(e.Id, e.UserId, e.RouteId);
    }
}