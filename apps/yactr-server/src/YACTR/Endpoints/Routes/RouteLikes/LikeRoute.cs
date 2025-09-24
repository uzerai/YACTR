using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;
using YACTR.Endpoints.Routes.RouteLikes.ViewModels;

namespace YACTR.Endpoints.Routes.RouteLikes;

public record LikeRouteRequest(Guid RouteId);

public class LikeRoute : Endpoint<LikeRouteRequest, RouteLikeResponse>
{
    private readonly IEntityRepository<RouteLike> _routeLikeRepository;
    private readonly IUserContext _userContext;

    public LikeRoute(
        IEntityRepository<RouteLike> routeLikeRepository,
        IUserContext userContext)
    {
        _routeLikeRepository = routeLikeRepository;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Post("/{RouteId}/like");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(LikeRouteRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;

        // Check if user already has a like for this route
        var existingLike = await _routeLikeRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(rl => rl.RouteId == req.RouteId && rl.UserId == currentUser.Id, ct);

        RouteLike routeLike;

        if (existingLike != null)
        {
            // User already liked this route, return the existing like
            routeLike = existingLike;
        }
        else
        {
            // Create a new like
            routeLike = await _routeLikeRepository.CreateAsync(new RouteLike
            {
                Id = Guid.NewGuid(),
                RouteId = req.RouteId,
                UserId = currentUser.Id
            }, ct);
        }

        await SendOkAsync(new RouteLikeResponse(
            Id: routeLike.Id,
            UserId: routeLike.UserId,
            RouteId: routeLike.RouteId
        ), cancellation: ct);
    }
}
