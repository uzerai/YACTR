using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Api.Endpoints.Ascents;

public record UpdateAscentData(
    AscentType Type,
    Instant CompletedAt
);

public class UpdateAscentRequest
{
    [BindFrom("ascent_id")]
    public required Guid AscentId { get; set; }

    [FromBody]
    public required UpdateAscentData Data { get; set; }
}

public class UpdateAscent : AuthenticatedEndpoint<UpdateAscentRequest, EmptyResponse>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Put("/{ascent_id}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateAscentRequest req, CancellationToken ct)
    {
        var ascent = await AscentRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(a => a.Id == req.AscentId, ct);

        if (ascent == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        // Ensure the user owns this ascent
        if (ascent.UserId != CurrentUserId)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        ascent.Type = req.Data.Type;
        ascent.CompletedAt = req.Data.CompletedAt;
        await AscentRepository.UpdateAsync(ascent, ct);
        await Send.NoContentAsync(ct);
    }
}
