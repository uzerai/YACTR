using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Pitches;

public record CreatePitchRequest(
    Guid SectorId,
    Guid RouteId,
    string? Name,
    ClimbingType Type,
    string? Description,
    string? Grade,
    int PitchOrder
);

public record CreatePitchResponse(
    Guid Id,
    Guid RouteId,
    Guid SectorId,
    string? Name,
    ClimbingType Type,
    string? Description,
    int? Grade,
    int? PitchOrder = null
);

public class CreatePitch : AuthenticatedEndpoint<CreatePitchRequest, CreatePitchResponse>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<PitchesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.PitchesWrite)));
    }

    public override async Task HandleAsync(CreatePitchRequest req, CancellationToken ct)
    {
        var newEntity = new Pitch
        {
            Name = req.Name,
            Type = req.Type,
            Description = req.Description,
            SectorId = req.SectorId,
            RouteId = req.RouteId,
            PitchOrder = req.PitchOrder,
        };

        var createdPitch = await PitchRepository.CreateAsync(newEntity, ct);

        await Send.CreatedAtAsync<GetPitchById>(
            createdPitch.Id,
            new CreatePitchResponse(
                createdPitch.Id,
                createdPitch.RouteId,
                createdPitch.SectorId,
                createdPitch.Name,
                createdPitch.Type,
                createdPitch.Description,
                createdPitch.Grade,
                createdPitch.PitchOrder
            ), cancellation: ct);
    }
}