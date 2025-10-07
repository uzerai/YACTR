using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Pitches;

public record UpdatePitchRequest(Guid PitchId);

public class UpdatePitch : AuthenticatedEndpoint<UpdatePitchRequest, EmptyResponse>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Put("/{PitchId}");
        Group<PitchesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.PitchesWrite)));
    }

    public override async Task HandleAsync(UpdatePitchRequest req, CancellationToken ct)
    {
        var existingPitch = await PitchRepository.GetByIdAsync(req.PitchId, ct);
        if (existingPitch == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Note: This would need to be implemented with proper request body handling
        // The original controller had incomplete update logic
        await PitchRepository.UpdateAsync(existingPitch, ct);

        await SendNoContentAsync(ct);
    }
}