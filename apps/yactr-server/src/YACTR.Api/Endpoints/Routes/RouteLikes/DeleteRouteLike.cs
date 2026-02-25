using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Climbing.Rating;
using YACTR.Infrastructure.Database.Repository.Interface;
using Route = YACTR.Domain.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Routes.RouteLikes;

public class DeleteRouteLike(IEntityRepository<RouteLike> routeLikeRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<RouteLikeRequest, RouteLikeResponse, RouteLikeDataMapper>
{
    public override void Configure()
    {
        Delete("/{RouteId}/like");
        Group<RoutesEndpointGroup>();
    }

    public override async Task<Void> HandleAsync(RouteLikeRequest req, CancellationToken ct)
    {
        if (await routeRepository.BuildReadonlyQuery()
            .FirstOrDefaultAsync(e => e.Id == req.RouteId, ct) is null)
        {
            return await Send.NotFoundAsync(ct);
        }

        // Check if user already has a like for this route
        var existingLike = await routeLikeRepository.BuildReadonlyQuery()
            .Where(rl => rl.RouteId == req.RouteId)
            .Where(rl => rl.UserId == CurrentUserId)
            .FirstOrDefaultAsync(ct);

        if (existingLike is not null)
        {
            await routeLikeRepository.DeleteAsync(existingLike, ct);
            return await Send.OkAsync(await Map.FromEntityAsync(existingLike, ct), ct);
        }

        return await Send.NotFoundAsync(ct);
    }
}
