using FastEndpoints;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Pitches;

public record GetPitchByIdRequest(Guid PitchId);

public record GetPitchByIdResponse(
    Guid Id,
    Guid RouteId,
    Guid SectorId,
    string? Name,
    ClimbingType Type,
    string? Description,
    int? Grade,
    int? PitchOrder = null
);

public class GetPitchById : Endpoint<GetPitchByIdRequest, GetPitchByIdResponse>
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

        await Send.OkAsync(new GetPitchByIdResponse(
            pitch.Id,
            pitch.RouteId,
            pitch.SectorId,
            pitch.Name,
            pitch.Type,
            pitch.Description,
            pitch.Grade,
            pitch.PitchOrder
        ), cancellation: ct);
    }
}