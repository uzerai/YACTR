using FastEndpoints;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints.Ascents;

public record GetAscentByIdRequest(Guid AscentId);

public class GetAscentById : Endpoint<GetAscentByIdRequest, AscentResponse>
{
    private readonly IRepository<Ascent> _ascentRepository;

    public GetAscentById(IRepository<Ascent> ascentRepository)
    {
        _ascentRepository = ascentRepository;
    }

    public override void Configure()
    {
        Get("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(GetAscentByIdRequest req, CancellationToken ct)
    {
        var ascent = await _ascentRepository.BuildReadonlyQuery()
            .Include(a => a.Route)
            .FirstOrDefaultAsync(a => a.Id == req.AscentId, ct);

        if (ascent == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new AscentResponse(
            Id: ascent.Id,
            UserId: ascent.UserId,
            Type: ascent.Type,
            CompletedAt: ascent.CompletedAt,
            Route: ascent.Route
        ), cancellation: ct);
    }
}