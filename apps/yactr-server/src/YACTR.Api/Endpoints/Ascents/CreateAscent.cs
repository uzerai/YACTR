using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Achievement;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Ascents;

public record CreateAscentRequest(
    Guid RouteId,
    AscentType Type,
    Instant CompletedAt
);

public record CreateAscentResponse(
  Guid Id,
  Guid UserId,
  AscentType Type,
  Instant CompletedAt,
  Route? Route
);

public class CreateAscent : AuthenticatedEndpoint<CreateAscentRequest, CreateAscentResponse>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(CreateAscentRequest req, CancellationToken ct)
    {
        var now = SystemClock.Instance.GetCurrentInstant();

        var createdAscent = await AscentRepository.CreateAsync(new Ascent
        {
            Id = Guid.NewGuid(),
            RouteId = req.RouteId,
            Type = req.Type,
            CompletedAt = req.CompletedAt,
            CreatedAt = now,
            UserId = CurrentUserId
        }, ct);

        // Reload the ascent with the Route navigation property
        var ascentWithRoute = await AscentRepository.BuildReadonlyQuery()
            .Include(a => a.Route)
            .FirstAsync(a => a.Id == createdAscent.Id, ct);

        await Send.CreatedAtAsync<GetAscentById>(ascentWithRoute.Id, new CreateAscentResponse(
            Id: ascentWithRoute.Id,
            UserId: ascentWithRoute.UserId,
            Type: ascentWithRoute.Type,
            CompletedAt: ascentWithRoute.CompletedAt,
            Route: ascentWithRoute.Route
        ), cancellation: ct);
    }
}
