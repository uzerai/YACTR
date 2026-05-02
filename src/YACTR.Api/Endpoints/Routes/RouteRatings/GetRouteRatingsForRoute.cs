using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using YACTR.Api.Endpoints.Routes.RouteRatings;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing.Rating;

using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class GetRouteRatingsForRouteRequest : PaginationRequest
{
    public Guid RouteId { get; init; }
}

public class GetRouteRatingsForRoute : Endpoint<GetRouteRatingsForRouteRequest, PaginatedResponse<RouteRatingResponse>, RouteRatingDataMapper>
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

        var likes = await RouteRatingRepository.AllAvailable()
            .AsNoTracking()
            .Where(e => e.RouteId == req.RouteId)
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);

        await Send.OkAsync(likes, cancellation: ct);
    }
}