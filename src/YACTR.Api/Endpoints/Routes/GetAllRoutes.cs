using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Infrastructure.Database.Repository.Interface;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class GetAllRoutesRequest : PaginationRequest {}

public class GetAllRoutes : Endpoint<GetAllRoutesRequest, PaginatedResponse<RouteResponse>, RouteDataMapper>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetAllRoutesRequest req, CancellationToken ct)
    {
        var routes = await RouteRepository
            .AllAvailable()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);

        await Send.OkAsync(routes, cancellation: ct);
    }
}