using Microsoft.EntityFrameworkCore;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;

using Route = YACTR.Domain.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Routes.RouteRatings;

public class DeleteRouteRatingRequest
{
    public Guid RouteId { get; set; }
}

public record DeleteRouteRatingResponse(
    Guid Id,
    Guid UserId,
    Guid RouteId,
    int Rating
);

public class DeleteRouteRating(IEntityRepository<RouteRating> routeRatingRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<DeleteRouteRatingRequest, DeleteRouteRatingResponse>
{
    public override void Configure()
    {
        Delete("/{route_id}/rating");
        Group<RoutesEndpointGroup>();
    }

    public override async Task<Void> HandleAsync(DeleteRouteRatingRequest req, CancellationToken ct)
    {
        if (await routeRepository.BuildReadonlyQuery()
            .FirstOrDefaultAsync(e => e.Id == req.RouteId, ct) is null)
        {
            return await Send.NotFoundAsync(ct);
        }

        // Check if user already has a like for this route
        var existingRating = await routeRatingRepository.BuildReadonlyQuery()
            .Where(rl => rl.RouteId == req.RouteId)
            .Where(rl => rl.UserId == CurrentUserId)
            .FirstOrDefaultAsync(ct);

        if (existingRating is not null)
        {
            await routeRatingRepository.DeleteAsync(existingRating, ct);
            return await Send.OkAsync(new DeleteRouteRatingResponse(existingRating.Id, existingRating.UserId, existingRating.RouteId, existingRating.Rating), ct);
        }

        return await Send.NotFoundAsync(ct);
    }
}
