using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Achievement;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Ascents;

public record GetAscentByIdRequest(Guid AscentId);

public record GetAscentByIdResponse(
  Guid Id,
  Guid UserId,
  AscentType Type,
  Instant CompletedAt,
  Route? Route
);

public class GetAscentById : Endpoint<GetAscentByIdRequest, GetAscentByIdResponse>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Get("/{ascent_id}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(GetAscentByIdRequest req, CancellationToken ct)
    {
        var ascent = await AscentRepository.BuildReadonlyQuery()
            .Include(a => a.Route)
            .FirstOrDefaultAsync(a => a.Id == req.AscentId, ct);

        if (ascent == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new GetAscentByIdResponse(
            Id: ascent.Id,
            UserId: ascent.UserId,
            Type: ascent.Type,
            CompletedAt: ascent.CompletedAt,
            Route: ascent.Route
        ), cancellation: ct);
    }
}