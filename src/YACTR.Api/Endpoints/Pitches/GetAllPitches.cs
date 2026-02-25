using FastEndpoints;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Pitches;

public class GetAllPitches : Endpoint<EmptyRequest, List<Pitch>>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        AllowAnonymous();
        Get("/");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var pitches = await PitchRepository.GetAllAsync(ct);
        await Send.OkAsync([.. pitches], cancellation: ct);
    }
}