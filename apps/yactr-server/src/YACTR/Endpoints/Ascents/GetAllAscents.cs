using FastEndpoints;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints;

public class GetAllAscents : Endpoint<EmptyRequest, List<AscentResponse>>
{
    private readonly IRepository<RouteAscent> _routeAscentRepository;
    private readonly IRepository<PitchAscent> _pitchAscentRepository;

    public GetAllAscents(IRepository<RouteAscent> routeAscentRepository, IRepository<PitchAscent> pitchAscentRepository)
    {
        _routeAscentRepository = routeAscentRepository;
        _pitchAscentRepository = pitchAscentRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {   
        var routeAscents = await _routeAscentRepository.BuildReadonlyQuery().ToListAsync(ct);
        var pitchAscents = await _pitchAscentRepository.BuildReadonlyQuery().ToListAsync(ct);

        // Combine both types of ascents into a single list
        var allAscents = routeAscents.Select(ra => new AscentResponse(
            Id: ra.Id,
            UserId: ra.UserId,
            Type: ra.Type,
            CompletedAt: ra.CompletedAt,
            Route: ra.Route,
            Pitch: null
        )).Concat(pitchAscents.Select(pa => new AscentResponse(
            Id: pa.Id,
            UserId: pa.UserId,
            Type: pa.Type,
            CompletedAt: pa.CompletedAt,
            Route: null,
            Pitch: pa.Pitch
        ))).ToList();

        // Order by completion date (most recent first)
        var orderedAscents = allAscents.OrderByDescending(a => a.CompletedAt).ToList();

        await SendAsync(orderedAscents, cancellation: ct);
    }
} 