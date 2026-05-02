using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Api.Endpoints.Routes.RouteLikes;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;

using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class GetRouteLikesForRouteRequest : PaginationRequest
{
    public Guid RouteId { get; init; }
}

public class GetRouteLikesForRoute : Endpoint<GetRouteLikesForRouteRequest, PaginatedResponse<RouteLikeResponse>, RouteLikeDataMapper>
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
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);

        await Send.OkAsync(likes, cancellation: ct);
    }
}