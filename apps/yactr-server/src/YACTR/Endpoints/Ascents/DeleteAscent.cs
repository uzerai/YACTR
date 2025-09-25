using FastEndpoints;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using FastEndpoints.Security;
using System.Security.Claims;

namespace YACTR.Endpoints.Ascents;

public record DeleteAscentRequest(Guid AscentId);

public class DeleteAscent : Endpoint<DeleteAscentRequest, AscentResponse>
{
    private readonly IRepository<Ascent> _ascentRepository;

    public DeleteAscent(
        IRepository<Ascent> ascentRepository)
    {
        _ascentRepository = ascentRepository;
    }

    public override void Configure()
    {
        Delete("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteAscentRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(HttpContext.User.ClaimValue(ClaimTypes.Sid), out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var ascent = await _ascentRepository.BuildTrackedQuery()
            .Include(a => a.Route)
            .FirstOrDefaultAsync(a => a.Id == req.AscentId, ct);

        if (ascent == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Ensure the user owns this ascent
        if (ascent.UserId != userId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await _ascentRepository.DeleteAsync(ascent, ct);
        await SendAsync(new AscentResponse(
            Id: ascent.Id,
            UserId: ascent.UserId,
            Type: ascent.Type,
            CompletedAt: ascent.CompletedAt,
            Route: ascent.Route
        ), cancellation: ct);
    }
}
