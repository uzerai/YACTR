using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using FastEndpoints.Security;
using System.Security.Claims;

namespace YACTR.Endpoints.Ascents;

public record UpdateAscentRequest(
    Guid AscentId,
    AscentType Type,
    Instant CompletedAt
);

public class UpdateAscent : Endpoint<UpdateAscentRequest, EmptyResponse>
{
    private readonly IRepository<Ascent> _ascentRepository;

    public UpdateAscent(IRepository<Ascent> ascentRepository)
    {
        _ascentRepository = ascentRepository;
    }

    public override void Configure()
    {
        Put("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateAscentRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(HttpContext.User.ClaimValue(ClaimTypes.Sid), out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
        }

        var ascent = await _ascentRepository.BuildTrackedQuery()
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

        ascent.Type = req.Type;
        ascent.CompletedAt = req.CompletedAt;
        await _ascentRepository.UpdateAsync(ascent, ct);
        await SendNoContentAsync(ct);
    }
}
