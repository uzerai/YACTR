using FastEndpoints;
using YACTR.Data.Repository.Interface;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public class GetAllRoutes : Endpoint<EmptyRequest, IEnumerable<RouteResponse>, RouteDataMapper>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var routes = await RouteRepository.GetAllAvailableAsync(ct);
        await SendAsync(await Task.WhenAll(routes.Select(async e => await Map.FromEntityAsync(e, ct))), cancellation: ct);
    }
}