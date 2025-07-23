using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;
using Route = YACTR.Data.Model.Location.Route;

namespace YACTR.Endpoints;

public record CreateAscentRequest(
    Guid? RouteId,
    Guid? PitchId,
    AscentType Type,
    Instant CompletedAt
);

public class CreateAscent : Endpoint<CreateAscentRequest, AscentResponse>
{
    private readonly IRepository<RouteAscent> _routeAscentRepository;
    private readonly IRepository<PitchAscent> _pitchAscentRepository;
    private readonly IUserContext _userContext;

    public CreateAscent(
        IRepository<RouteAscent> routeAscentRepository,
        IRepository<PitchAscent> pitchAscentRepository,
        IUserContext userContext)
    {
        _routeAscentRepository = routeAscentRepository;
        _pitchAscentRepository = pitchAscentRepository;
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

        BaseAscent createdAscent;

        if (req.RouteId.HasValue)
        {
            createdAscent = await _routeAscentRepository.CreateAsync(new RouteAscent
            {
                Id = Guid.NewGuid(),
                RouteId = req.RouteId.Value,
                Type = req.Type,
                CompletedAt = req.CompletedAt,
                CreatedAt = now,
                UserId = currentUser.Id
            }, ct);
        }
        else if (req.PitchId.HasValue)
        {
            createdAscent = await _pitchAscentRepository.CreateAsync(new PitchAscent
            {
                Id = Guid.NewGuid(),
                PitchId = req.PitchId.Value,
                Type = req.Type,
                CompletedAt = req.CompletedAt,
                CreatedAt = now,
                UserId = currentUser.Id
            }, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        await SendCreatedAtAsync<GetAscentById>(createdAscent.Id, new AscentResponse(
            Id: createdAscent.Id,
            UserId: createdAscent.UserId,
            Type: createdAscent.Type,
            CompletedAt: createdAscent.CompletedAt,
            Route: createdAscent is RouteAscent routeAscent ? routeAscent.Route : null,
            Pitch: createdAscent is PitchAscent pitchAscent ? pitchAscent.Pitch : null
        ), cancellation: ct);
    }
}
