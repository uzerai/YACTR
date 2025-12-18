using FastEndpoints;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Pitches;

public record GetPitchByIdRequest(Guid PitchId);

public class GetPitchById : Endpoint<GetPitchByIdRequest, Pitch>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Get("/{PitchId}");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(GetPitchByIdRequest req, CancellationToken ct)
    {
        var pitch = await PitchRepository.GetByIdAsync(req.PitchId, ct);

        if (pitch == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(pitch, cancellation: ct);
    }
}