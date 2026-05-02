using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;

using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class GetRouteLikesForRouteRequest : PaginationRequest
{
    public Guid RouteId { get; init; }
}

public record GetRouteLikesForRouteResponseItem(
    Guid Id,
    Guid UserId,
    Guid RouteId
);

public class GetRouteLikesForRoute : Endpoint<GetRouteLikesForRouteRequest, PaginatedResponse<GetRouteLikesForRouteResponseItem>>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }
    public required IEntityRepository<RouteLike> RouteLikeRepository { get; init; }

    public override void Configure()
    {
        Get("/{route_id}/likes");
        AllowAnonymous();
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetRouteLikesForRouteRequest req, CancellationToken ct)
    {
        var route = await RouteRepository.AllAvailable()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == req.RouteId, ct);

        if (route == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var likes = await RouteLikeRepository.AllAvailable()
            .AsNoTracking()
            .Where(e => e.RouteId == req.RouteId)
            .ToPaginatedResponseAsync(MapRouteLikeAsync, req, ct);

        await Send.OkAsync(likes, cancellation: ct);
    }

    private static Task<GetRouteLikesForRouteResponseItem> MapRouteLikeAsync(RouteLike routeLike, CancellationToken ct)
    {
        return Task.FromResult(new GetRouteLikesForRouteResponseItem(routeLike.Id, routeLike.UserId, routeLike.RouteId));
    }
}