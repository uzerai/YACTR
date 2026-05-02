using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Pitches;

public class GetAllPitchesRequest : PaginationRequest { }

public record GetAllPitchesResponseItem(
    Guid Id,
    Guid RouteId,
    Guid SectorId,
    string? Name,
    ClimbingType Type,
    string? Description,
    int? Grade,
    int? PitchOrder = null
);

public class GetAllPitches : Endpoint<GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>
{
    public required IEntityRepository<Pitch> PitchRepository { get; init; }

    public override void Configure()
    {
        AllowAnonymous();
        Get("/");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(GetAllPitchesRequest req, CancellationToken ct)
    {
        var pitches = await PitchRepository.AllAvailable()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToPaginatedResponseAsync(MapPitchToResponseAsync, req, ct);

        await Send.OkAsync(pitches, cancellation: ct);
    }

    private static Task<GetAllPitchesResponseItem> MapPitchToResponseAsync(Pitch pitch, CancellationToken ct)
    {
        return Task.FromResult(new GetAllPitchesResponseItem(
            pitch.Id,
            pitch.RouteId,
            pitch.SectorId,
            pitch.Name,
            pitch.Type,
            pitch.Description,
            pitch.Grade,
            pitch.PitchOrder
        ));
    }
}