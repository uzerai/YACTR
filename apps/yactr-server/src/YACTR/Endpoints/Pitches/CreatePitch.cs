using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Pitches;

public class CreatePitch : AuthenticatedEndpoint<PitchRequestData, Pitch>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<PitchesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.PitchesWrite)));
    }

    public override async Task HandleAsync(PitchRequestData req, CancellationToken ct)
    {
        var createdPitch = await PitchRepository.CreateAsync(new()
        {
            Name = req.Name,
            Description = req.Description,
            Type = req.Type,
            SectorId = req.SectorId,
            RouteId = req.RouteId
        }, ct);

        await Send.CreatedAtAsync<GetPitchById>(createdPitch.Id, createdPitch, cancellation: ct);
    }
}