using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Pitches;

public class GetAllPitches : Endpoint<EmptyRequest, List<Pitch>>
{
    private readonly IEntityRepository<Pitch> _pitchRepository;

    public GetAllPitches(IEntityRepository<Pitch> pitchRepository)
    {
        _pitchRepository = pitchRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var pitches = await _pitchRepository.GetAllAsync(ct);
        await SendAsync([.. pitches], cancellation: ct);
    }
}