using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Pitches;

public record GetPitchByIdRequest(Guid PitchId);

public class GetPitchById : Endpoint<GetPitchByIdRequest, Pitch>
{
    private readonly IEntityRepository<Pitch> _pitchRepository;

    public GetPitchById(IEntityRepository<Pitch> pitchRepository)
    {
        _pitchRepository = pitchRepository;
    }

    public override void Configure()
    {
        Get("/{PitchId}");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(GetPitchByIdRequest req, CancellationToken ct)
    {
        var pitch = await _pitchRepository.GetByIdAsync(req.PitchId, ct);

        if (pitch == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(pitch, cancellation: ct);
    }
} 