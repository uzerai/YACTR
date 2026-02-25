using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Climbing.Rating;
using YACTR.Infrastructure.Database.Repository.Interface;
using Route = YACTR.Domain.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Routes.RouteRatings;

public class DeleteRouteRatingRequest
{
    public Guid RouteId { get; set; }
}

public class DeleteRouteRating(IEntityRepository<RouteRating> routeRatingRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<DeleteRouteRatingRequest, RouteRatingResponse, RouteRatingDataMapper>
{
    public override void Configure()
    {
        Delete("/{RouteId}/rating");
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
            return await Send.OkAsync(await Map.FromEntityAsync(existingRating, ct), ct);
        }

        return await Send.NotFoundAsync(ct);
    }
}
