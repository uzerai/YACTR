using Microsoft.EntityFrameworkCore;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;

using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes.RouteLikes;

public record CreateRouteLikeRequest(Guid RouteId);

public record CreateRouteLikeResponse(
    Guid Id,
    Guid UserId,
    Guid RouteId
);

public class CreateRouteLike(IEntityRepository<RouteLike> routeLikeRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<CreateRouteLikeRequest, CreateRouteLikeResponse>
{
    public override void Configure()
    {
        Post("/{route_id}/like");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(CreateRouteLikeRequest req, CancellationToken ct)
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
            await Send.OkAsync(new CreateRouteLikeResponse(existingLike.Id, existingLike.UserId, existingLike.RouteId), ct);
            return;
        }

        var newLike = await routeLikeRepository.CreateAsync(new()
        {
            UserId = CurrentUserId,
            RouteId = req.RouteId
        });

        await Send.OkAsync(new CreateRouteLikeResponse(newLike.Id, newLike.UserId, newLike.RouteId), ct);
    }
}
