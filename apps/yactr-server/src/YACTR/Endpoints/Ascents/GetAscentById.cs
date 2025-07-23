using FastEndpoints;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints;

public record GetAscentByIdRequest(Guid AscentId);

public class GetAscentById : Endpoint<GetAscentByIdRequest, AscentResponse>
{
    private readonly IRepository<RouteAscent> _routeAscentRepository;
    private readonly IRepository<PitchAscent> _pitchAscentRepository;

    public GetAscentById(IRepository<RouteAscent> routeAscentRepository, IRepository<PitchAscent> pitchAscentRepository)
    {
        _routeAscentRepository = routeAscentRepository;
        _pitchAscentRepository = pitchAscentRepository;
    }

    public override void Configure()
    {
        Get("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(GetAscentByIdRequest req, CancellationToken ct)
    {
        // Try to find the ascent as a RouteAscent first
        var routeAscent = await _routeAscentRepository.BuildReadonlyQuery()
            .FirstOrDefaultAsync(ra => ra.Id == req.AscentId, ct);
        if (routeAscent != null)
        {
            await SendAsync(new AscentResponse(
                Id: routeAscent.Id,
                UserId: routeAscent.UserId,
                Type: routeAscent.Type,
                CompletedAt: routeAscent.CompletedAt,
                Route: routeAscent.Route,
                Pitch: null
            ), cancellation: ct);
            return;
        }

        // Try to find the ascent as a PitchAscent
        var pitchAscent = await _pitchAscentRepository.BuildReadonlyQuery()
            .FirstOrDefaultAsync(pa => pa.Id == req.AscentId, ct);
        if (pitchAscent != null)
        {
            await SendAsync(new AscentResponse(
                Id: pitchAscent.Id,
                UserId: pitchAscent.UserId,
                Type: pitchAscent.Type,
                CompletedAt: pitchAscent.CompletedAt,
                Route: null,
                Pitch: pitchAscent.Pitch
            ), cancellation: ct);
            return;
        }

        await SendNotFoundAsync(ct);
    }
} 