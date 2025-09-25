using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
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

public class CreateAscent : Endpoint<CreateAscentRequest, AscentResponse>
{
    private readonly IRepository<Ascent> _ascentRepository;

    public CreateAscent(IRepository<Ascent> ascentRepository)
    {
        _ascentRepository = ascentRepository;
    }

    public override void Configure()
    {
        Post("/");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(CreateAscentRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(HttpContext.User.ClaimValue(ClaimTypes.Sid), out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var now = SystemClock.Instance.GetCurrentInstant();

        var createdAscent = await _ascentRepository.CreateAsync(new Ascent
        {
            Id = Guid.NewGuid(),
            RouteId = req.RouteId,
            Type = req.Type,
            CompletedAt = req.CompletedAt,
            CreatedAt = now,
            UserId = userId
        }, ct);

        // Reload the ascent with the Route navigation property
        var ascentWithRoute = await _ascentRepository.BuildReadonlyQuery()
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
