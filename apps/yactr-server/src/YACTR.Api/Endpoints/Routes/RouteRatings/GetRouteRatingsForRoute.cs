using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;

using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class GetRouteRatingsForRouteRequest : PaginationRequest
{
    public Guid RouteId { get; init; }
}

public record GetRouteRatingsForRouteResponseItem(
    Guid Id,
    Guid UserId,
    Guid RouteId,
    int Rating
);

public class GetRouteRatingsForRoute : Endpoint<GetRouteRatingsForRouteRequest, PaginatedResponse<GetRouteRatingsForRouteResponseItem>>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }
    public required IEntityRepository<RouteRating> RouteRatingRepository { get; init; }

    public override void Configure()
    {
        Get("/{route_id}/ratings");
        AllowAnonymous();
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetRouteRatingsForRouteRequest req, CancellationToken ct)
    {
        var route = await RouteRepository.AllAvailable()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == req.RouteId, ct);

        if (route == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var ratings = await RouteRatingRepository.AllAvailable()
            .AsNoTracking()
            .Where(e => e.RouteId == req.RouteId)
            .ToPaginatedResponseAsync(MapRouteRatingAsync, req, ct);

        await Send.OkAsync(ratings, cancellation: ct);
    }

    private static Task<GetRouteRatingsForRouteResponseItem> MapRouteRatingAsync(RouteRating routeRating, CancellationToken ct)
    {
        return Task.FromResult(new GetRouteRatingsForRouteResponseItem(routeRating.Id, routeRating.UserId, routeRating.RouteId, routeRating.Rating));
    }
}