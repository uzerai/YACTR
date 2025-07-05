using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;
using YACTR.DTO.RequestData;

namespace YACTR.Endpoints;

public record UpdatePitchRequest(Guid PitchId);

public class UpdatePitch : Endpoint<UpdatePitchRequest, EmptyResponse>
{
    private readonly IEntityRepository<Pitch> _pitchRepository;

    public UpdatePitch(IEntityRepository<Pitch> pitchRepository)
    {
        _pitchRepository = pitchRepository;
    }

    public override void Configure()
    {
        Put("/{PitchId}");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(UpdatePitchRequest req, CancellationToken ct)
    {
        var existingPitch = await _pitchRepository.GetByIdAsync(req.PitchId, ct);
        if (existingPitch == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Note: This would need to be implemented with proper request body handling
        // The original controller had incomplete update logic
        await _pitchRepository.UpdateAsync(existingPitch, ct);
        
        await SendNoContentAsync(ct);
    }
} 