using FastEndpoints;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Pitches;

public record GetPitchByIdRequest(Guid PitchId);

public class GetPitchById : Endpoint<GetPitchByIdRequest, PitchResponse, PitchDataMapper>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Get("/{pitch_id}");
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

        await Send.OkAsync(await Map.FromEntityAsync(pitch, ct), cancellation: ct);
    }
}