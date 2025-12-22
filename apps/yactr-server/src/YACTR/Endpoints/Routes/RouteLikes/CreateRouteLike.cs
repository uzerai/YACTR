using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Repository.Interface;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes.RouteLikes;

public class CreateRouteLike(IEntityRepository<RouteLike> routeLikeRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<RouteLikeRequest, RouteLikeResponse, RouteLikeDataMapper>
{
    public override void Configure()
    {
        Post("/{RouteId}/like");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(RouteLikeRequest req, CancellationToken ct)
    {
        if (await routeRepository.BuildReadonlyQuery()
            .FirstOrDefaultAsync(e => e.Id == req.RouteId, ct) is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        // Check if user already has a like for this route
        var existingLike = await routeLikeRepository.BuildReadonlyQuery()
            .Where(rl => rl.RouteId == req.RouteId)
            .Where(rl => rl.UserId == CurrentUserId)
            .FirstOrDefaultAsync(ct);

        if (existingLike is not null)
        {
            await Send.OkAsync(await Map.FromEntityAsync(existingLike, ct), ct);
            return;
        }

        var newLike = await routeLikeRepository.CreateAsync(new()
        {
            UserId = CurrentUserId,
            RouteId = req.RouteId
        });

        await Send.OkAsync(await Map.FromEntityAsync(newLike, ct), ct);
    }
}
