using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Achievement;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Ascents;

public record DeleteAscentRequest(Guid AscentId);

public record DeleteAscentResponse(
  Guid Id,
  Guid UserId,
  AscentType Type,
  Instant CompletedAt,
  Route? Route
);

public class DeleteAscent : AuthenticatedEndpoint<DeleteAscentRequest, DeleteAscentResponse>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Delete("/{ascent_id}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteAscentRequest req, CancellationToken ct)
    {
        var ascent = await AscentRepository.BuildTrackedQuery()
            .Include(a => a.Route)
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

        await AscentRepository.DeleteAsync(ascent, ct);
        await Send.OkAsync(new DeleteAscentResponse(
            Id: ascent.Id,
            UserId: ascent.UserId,
            Type: ascent.Type,
            CompletedAt: ascent.CompletedAt,
            Route: ascent.Route
        ), cancellation: ct);
    }
}
