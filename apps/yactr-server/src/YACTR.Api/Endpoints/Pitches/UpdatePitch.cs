using FastEndpoints;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Pitches;

public record UpdatePitchRequest(Guid PitchId, PitchRequestData Pitch);

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
            await Send.NotFoundAsync(ct);
            return;
        }

        existingPitch.Name = req.Pitch.Name;
        existingPitch.Description = req.Pitch.Description;
        existingPitch.Type = req.Pitch.Type;
        existingPitch.PitchOrder = req.Pitch.PitchOrder ?? existingPitch.PitchOrder;

        await PitchRepository.UpdateAsync(existingPitch, ct);

        await Send.NoContentAsync(ct);
    }
}