using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public record DeletePitchRequest(Guid PitchId);

public class DeletePitch : Endpoint<DeletePitchRequest, EmptyResponse>
{
    private readonly IEntityRepository<Pitch> _pitchRepository;

    public DeletePitch(IEntityRepository<Pitch> pitchRepository)
    {
        _pitchRepository = pitchRepository;
    }

    public override void Configure()
    {
        Delete("/{PitchId}");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(DeletePitchRequest req, CancellationToken ct)
    {
        var pitch = await _pitchRepository.GetByIdAsync(req.PitchId, ct);
        
        if (pitch == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await _pitchRepository.DeleteAsync(pitch, ct);
        await SendNoContentAsync(ct);
    }
} 