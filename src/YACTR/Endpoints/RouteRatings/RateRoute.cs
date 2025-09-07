using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.Endpoints;

public record RateRouteRequest(Guid RouteId, int Rating);

public class RateRoute : Endpoint<RateRouteRequest, RouteRatingResponse>
{
    private readonly IEntityRepository<RouteRating> _routeRatingRepository;
    private readonly IUserContext _userContext;

    public RateRoute(
        IEntityRepository<RouteRating> routeRatingRepository,
        IUserContext userContext)
    {
        _routeRatingRepository = routeRatingRepository;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Post("/{RouteId}/rating");
        Group<RouteRatingsEndpointGroup>();
    }

    public override async Task HandleAsync(RateRouteRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;

        // Validate rating value (assuming 1-5 scale)
        if (req.Rating < 1 || req.Rating > 5)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        // Check if user already has a rating for this route
        var existingRating = await _routeRatingRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(rr => rr.RouteId == req.RouteId && rr.UserId == currentUser.Id, ct);

        RouteRating routeRating;

        if (existingRating != null)
        {
            // Update existing rating
            existingRating.Rating = req.Rating;
            await _routeRatingRepository.UpdateAsync(existingRating, ct);
            routeRating = existingRating;
        }
        else
        {
            // Create a new rating
            routeRating = await _routeRatingRepository.CreateAsync(new RouteRating
            {
                Id = Guid.NewGuid(),
                RouteId = req.RouteId,
                UserId = currentUser.Id,
                Rating = req.Rating
            }, ct);
        }

        await SendOkAsync(new RouteRatingResponse(
            Id: routeRating.Id,
            UserId: routeRating.UserId,
            RouteId: routeRating.RouteId,
            Rating: routeRating.Rating
        ), cancellation: ct);
    }
}
