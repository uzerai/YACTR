using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Repository.Interface;
using Route = YACTR.Data.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Endpoints.Routes.RouteRatings;

public class CreateOrUpdateRouteRatingRequest
{
    public required Guid RouteId { get; set; }
    [FromBody]
    public required RouteRatingRequestData RatingData { get; set; }
}

public class CreateOrUpdateRouteRating(IEntityRepository<RouteRating> routeRatingRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<CreateOrUpdateRouteRatingRequest, RouteRatingResponse, RouteRatingDataMapper>
{
    public override void Configure()
    {
        Post("/{RouteId}/rating");
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
            var updatedRating = await Map.UpdateEntityAsync(req.RatingData, existingRouting, ct);
            await routeRatingRepository.UpdateAsync(updatedRating, ct);

            return await Send.OkAsync(await Map.FromEntityAsync(updatedRating, ct), ct);
        }

        var newRating = await routeRatingRepository.CreateAsync(new()
        {
            RouteId = req.RouteId,
            UserId = CurrentUserId,
            Rating = req.RatingData.Rating
        }, ct);

        return await Send.OkAsync(await Map.FromEntityAsync(newRating, ct), ct);
    }
}
