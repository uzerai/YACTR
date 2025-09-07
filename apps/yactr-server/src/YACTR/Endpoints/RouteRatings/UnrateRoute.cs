using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.Endpoints;

public record UnrateRouteRequest(Guid RouteId);

public class UnrateRoute : Endpoint<UnrateRouteRequest, RouteRatingResponse>
{
    private readonly IEntityRepository<RouteRating> _routeRatingRepository;
    private readonly IUserContext _userContext;

    public UnrateRoute(
        IEntityRepository<RouteRating> routeRatingRepository,
        IUserContext userContext)
    {
        _routeRatingRepository = routeRatingRepository;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Delete("/{RouteId}/rating");
        Group<RouteRatingsEndpointGroup>();
    }

    public override async Task HandleAsync(UnrateRouteRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;

        // Find the user's rating for this route
        var existingRating = await _routeRatingRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(rr => rr.RouteId == req.RouteId && rr.UserId == currentUser.Id, ct);

        if (existingRating == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Delete the rating
        await _routeRatingRepository.DeleteAsync(existingRating, ct);

        await SendOkAsync(new RouteRatingResponse(
            Id: existingRating.Id,
            UserId: existingRating.UserId,
            RouteId: existingRating.RouteId,
            Rating: existingRating.Rating
        ), cancellation: ct);
    }
}
