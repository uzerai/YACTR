using FastEndpoints;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints.Ascents;

public class GetAllAscents : Endpoint<EmptyRequest, List<AscentResponse>>
{
    private readonly IRepository<Ascent> _ascentRepository;

    public GetAllAscents(IRepository<Ascent> ascentRepository)
    {
        _ascentRepository = ascentRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<AscentsEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var ascents = await _ascentRepository.BuildReadonlyQuery()
            .Include(a => a.Route)
            .ToListAsync(ct);

        var ascentResponses = ascents.Select(a => new AscentResponse(
            Id: a.Id,
            UserId: a.UserId,
            Type: a.Type,
            CompletedAt: a.CompletedAt,
            Route: a.Route
        )).OrderByDescending(a => a.CompletedAt).ToList();

        await SendAsync(ascentResponses, cancellation: ct);
    }
}