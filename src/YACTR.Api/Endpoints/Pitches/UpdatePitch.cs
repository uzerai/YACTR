using FastEndpoints;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Pitches;

public record UpdatePitchData(
    Guid SectorId,
    Guid RouteId,
    string? Name,
    ClimbingType Type,
    string? Description,
    string? Grade,
    int PitchOrder
);

public class UpdatePitchRequest
{
    [BindFrom("pitch_id")]
    public required Guid PitchId { get; set; }

    [FromBody]
    public required UpdatePitchData Pitch { get; set; }
}

public class UpdatePitch : AuthenticatedEndpoint<UpdatePitchRequest, EmptyResponse>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Put("/{pitch_id}");
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
        existingPitch.PitchOrder = req.Pitch.PitchOrder;

        await PitchRepository.UpdateAsync(existingPitch, ct);

        await Send.NoContentAsync(ct);
    }
}