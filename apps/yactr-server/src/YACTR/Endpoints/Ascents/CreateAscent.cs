using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Ascents;

public record CreateAscentRequest(
    Guid RouteId,
    AscentType Type,
    Instant CompletedAt
);

public class CreateAscent : AuthenticatedEndpoint<CreateAscentRequest, AscentResponse>
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

        await SendCreatedAtAsync<GetAscentById>(ascentWithRoute.Id, new AscentResponse(
            Id: ascentWithRoute.Id,
            UserId: ascentWithRoute.UserId,
            Type: ascentWithRoute.Type,
            CompletedAt: ascentWithRoute.CompletedAt,
            Route: ascentWithRoute.Route
        ), cancellation: ct);
    }
}
