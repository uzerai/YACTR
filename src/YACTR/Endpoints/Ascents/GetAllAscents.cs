using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Ascents;

public class GetAllAscents : Endpoint<EmptyRequest, List<AscentResponse>>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<AscentsEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var ascents = await AscentRepository.BuildReadonlyQuery()
            .Include(a => a.Route)
            .ToListAsync(ct);

        var ascentResponses = ascents.Select(a => new AscentResponse(
            Id: a.Id,
            UserId: a.UserId,
            Type: a.Type,
            CompletedAt: a.CompletedAt,
            Route: a.Route
        )).OrderByDescending(a => a.CompletedAt).ToList();

        await Send.OkAsync(ascentResponses, cancellation: ct);
    }
}