using FastEndpoints;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Pitches;

public record DeletePitchRequest(Guid PitchId);

public class DeletePitch : AuthenticatedEndpoint<DeletePitchRequest, EmptyResponse>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Delete("/{pitch_id}");
        Group<PitchesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.PitchesWrite)));
    }

    public override async Task HandleAsync(DeletePitchRequest req, CancellationToken ct)
    {
        var pitch = await PitchRepository.GetByIdAsync(req.PitchId, ct);

        if (pitch == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await PitchRepository.DeleteAsync(pitch, ct);
        await Send.NoContentAsync(ct);
    }
}