using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;
using Route = YACTR.Domain.Model.Climbing.Route;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Routes.RouteLikes;

public class DeleteRouteLikeRequest
{
    [BindFrom("route_id")]
    public required Guid RouteId { get; set; }
}

public record DeleteRouteLikeResponse(
    Guid Id,
    Guid UserId,
    Guid RouteId
);

public class DeleteRouteLike(IEntityRepository<RouteLike> routeLikeRepository, IEntityRepository<Route> routeRepository) : AuthenticatedEndpoint<DeleteRouteLikeRequest, DeleteRouteLikeResponse>
{
    public override void Configure()
    {
        Delete("/{route_id}/like");
        Group<RoutesEndpointGroup>();
    }

    public override async Task<Void> HandleAsync(DeleteRouteLikeRequest req, CancellationToken ct)
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
            return await Send.OkAsync(new DeleteRouteLikeResponse(existingLike.Id, existingLike.UserId, existingLike.RouteId), ct);
        }

        return await Send.NotFoundAsync(ct);
    }
}
