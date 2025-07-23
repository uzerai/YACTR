using FastEndpoints;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints;

public record DeleteAscentRequest(Guid AscentId);

public class DeleteAscent : Endpoint<DeleteAscentRequest, AscentResponse>
{
    private readonly IRepository<RouteAscent> _routeAscentRepository;
    private readonly IRepository<PitchAscent> _pitchAscentRepository;
    private readonly IUserContext _userContext;

    public DeleteAscent(
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
        Delete("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteAscentRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;

        // Try to find the ascent as a RouteAscent first
        var routeAscent = await _routeAscentRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(ra => ra.Id == req.AscentId, ct);
        if (routeAscent != null)
        {
            // Ensure the user owns this ascent
            if (routeAscent.UserId != currentUser.Id)
            {
                await SendForbiddenAsync(ct);
                return;
            }

            await _routeAscentRepository.DeleteAsync(routeAscent, ct);
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
        var pitchAscent = await _pitchAscentRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(pa => pa.Id == req.AscentId, ct);
        if (pitchAscent != null)
        {
            // Ensure the user owns this ascent
            if (pitchAscent.UserId != currentUser.Id)
            {
                await SendForbiddenAsync(ct);
                return;
            }

            await _pitchAscentRepository.DeleteAsync(pitchAscent, ct);
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
