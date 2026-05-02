using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;

using Route = YACTR.Domain.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Routes.RouteRatings;

public record CreateOrUpdateRouteRatingData(int Rating);

public class CreateOrUpdateRouteRatingRequest
{
    public required Guid RouteId { get; set; }
    [FromBody]
    public required CreateOrUpdateRouteRatingData RatingData { get; set; }
}

public record CreateOrUpdateRouteRatingResponse(
    Guid Id,
    Guid UserId,
    Guid RouteId,
    int Rating
);

public class CreateOrUpdateRouteRating(IEntityRepository<RouteRating> routeRatingRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<CreateOrUpdateRouteRatingRequest, CreateOrUpdateRouteRatingResponse>
{
    public override void Configure()
    {
        Post("/{route_id}/rating");
        Group<RoutesEndpointGroup>();
    }

    public override async Task<Void> HandleAsync(CreateOrUpdateRouteRatingRequest req, CancellationToken ct)
    {
        if (await routeRepository.BuildReadonlyQuery()
            .FirstOrDefaultAsync(e => e.Id == req.RouteId, ct) is null)
        {
            return await Send.NotFoundAsync(ct);
        }

        // Check if user already has a like for this route
        var existingRouting = await routeRatingRepository.BuildReadonlyQuery()
            .Where(rl => rl.RouteId == req.RouteId)
            .Where(rl => rl.UserId == CurrentUserId)
            .FirstOrDefaultAsync(ct);

        if (existingRouting is not null)
        {
            existingRouting.Rating = req.RatingData.Rating;
            var updatedRating = existingRouting;
            await routeRatingRepository.UpdateAsync(updatedRating, ct);

            return await Send.OkAsync(new CreateOrUpdateRouteRatingResponse(updatedRating.Id, updatedRating.UserId, updatedRating.RouteId, updatedRating.Rating), ct);
        }

        var newRating = await routeRatingRepository.CreateAsync(new()
        {
            RouteId = req.RouteId,
            UserId = CurrentUserId,
            Rating = req.RatingData.Rating
        }, ct);

        return await Send.OkAsync(new CreateOrUpdateRouteRatingResponse(newRating.Id, newRating.UserId, newRating.RouteId, newRating.Rating), ct);
    }
}
