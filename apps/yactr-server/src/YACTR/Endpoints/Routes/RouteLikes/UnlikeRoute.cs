using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;
using YACTR.Endpoints.Routes.RouteLikes.ViewModels;

namespace YACTR.Endpoints.Routes.RouteLikes;

public record UnlikeRouteRequest(Guid RouteId);

public class UnlikeRoute : Endpoint<UnlikeRouteRequest, RouteLikeResponse>
{
    private readonly IEntityRepository<RouteLike> _routeLikeRepository;
    private readonly IUserContext _userContext;

    public UnlikeRoute(
        IEntityRepository<RouteLike> routeLikeRepository,
        IUserContext userContext)
    {
        _routeLikeRepository = routeLikeRepository;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Delete("/{RouteId}/like");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(UnlikeRouteRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;

        // Find the user's like for this route
        var existingLike = await _routeLikeRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(rl => rl.RouteId == req.RouteId && rl.UserId == currentUser.Id, ct);

        if (existingLike == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Delete the like
        await _routeLikeRepository.DeleteAsync(existingLike, ct);

        await SendOkAsync(new RouteLikeResponse(
            Id: existingLike.Id,
            UserId: existingLike.UserId,
            RouteId: existingLike.RouteId
        ), cancellation: ct);
    }
}
