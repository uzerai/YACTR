using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Ascents;

public record GetAscentByIdRequest(Guid AscentId);

public class GetAscentById : Endpoint<GetAscentByIdRequest, AscentResponse>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Get("/{AscentId}");
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

        await Send.OkAsync(new AscentResponse(
            Id: ascent.Id,
            UserId: ascent.UserId,
            Type: ascent.Type,
            CompletedAt: ascent.CompletedAt,
            Route: ascent.Route
        ), cancellation: ct);
    }
}