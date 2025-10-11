using FastEndpoints;
using YACTR.Data.Repository.Interface;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Endpoints.Routes;

public class GetAllRoutes : Endpoint<EmptyRequest, List<Route>>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var routes = await RouteRepository.GetAllAsync(ct);
        await SendAsync([.. routes], cancellation: ct);
    }
}