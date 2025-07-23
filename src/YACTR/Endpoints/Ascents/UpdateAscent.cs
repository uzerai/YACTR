using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints;

public record UpdateAscentRequest(
    Guid AscentId,
    AscentType Type,
    Instant CompletedAt
);

public class UpdateAscent : Endpoint<UpdateAscentRequest, EmptyResponse>
{
    private readonly IRepository<RouteAscent> _routeAscentRepository;
    private readonly IRepository<PitchAscent> _pitchAscentRepository;
    private readonly IUserContext _userContext;

    public UpdateAscent(
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
        Put("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateAscentRequest req, CancellationToken ct)
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

            routeAscent.Type = req.Type;
            routeAscent.CompletedAt = req.CompletedAt;
            await _routeAscentRepository.UpdateAsync(routeAscent, ct);
            await SendNoContentAsync(ct);
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

            pitchAscent.Type = req.Type;
            pitchAscent.CompletedAt = req.CompletedAt;
            await _pitchAscentRepository.UpdateAsync(pitchAscent, ct);
            await SendNoContentAsync(ct);
            return;
        }

        await SendNotFoundAsync(ct);
    }
}
