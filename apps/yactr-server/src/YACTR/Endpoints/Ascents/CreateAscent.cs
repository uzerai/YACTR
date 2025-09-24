using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.Endpoints.Ascents;

public record CreateAscentRequest(
    Guid RouteId,
    AscentType Type,
    Instant CompletedAt
);

public class CreateAscent : Endpoint<CreateAscentRequest, AscentResponse>
{
    private readonly IRepository<Ascent> _ascentRepository;
    private readonly IUserContext _userContext;

    public CreateAscent(
        IRepository<Ascent> ascentRepository,
        IUserContext userContext)
    {
        _ascentRepository = ascentRepository;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Post("/");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(CreateAscentRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;
        var now = SystemClock.Instance.GetCurrentInstant();

        var createdAscent = await _ascentRepository.CreateAsync(new Ascent
        {
            Id = Guid.NewGuid(),
            RouteId = req.RouteId,
            Type = req.Type,
            CompletedAt = req.CompletedAt,
            CreatedAt = now,
            UserId = currentUser.Id
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
