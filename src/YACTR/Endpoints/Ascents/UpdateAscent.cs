using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Ascents;

public record UpdateAscentRequest(
    Guid AscentId,
    AscentType Type,
    Instant CompletedAt
);

public class UpdateAscent : AuthenticatedEndpoint<UpdateAscentRequest, EmptyResponse>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Put("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateAscentRequest req, CancellationToken ct)
    {
        var ascent = await AscentRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(a => a.Id == req.AscentId, ct);

        if (ascent == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Ensure the user owns this ascent
        if (ascent.UserId != CurrentUserId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        ascent.Type = req.Type;
        ascent.CompletedAt = req.CompletedAt;
        await AscentRepository.UpdateAsync(ascent, ct);
        await SendNoContentAsync(ct);
    }
}
