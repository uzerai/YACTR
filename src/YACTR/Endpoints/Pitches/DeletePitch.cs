using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Pitches;

public record DeletePitchRequest(Guid PitchId);

public class DeletePitch : AuthenticatedEndpoint<DeletePitchRequest, EmptyResponse>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Delete("/{PitchId}");
        Group<PitchesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.PitchesWrite)));
    }

    public override async Task HandleAsync(DeletePitchRequest req, CancellationToken ct)
    {
        var pitch = await PitchRepository.GetByIdAsync(req.PitchId, ct);

        if (pitch == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await PitchRepository.DeleteAsync(pitch, ct);
        await SendNoContentAsync(ct);
    }
}