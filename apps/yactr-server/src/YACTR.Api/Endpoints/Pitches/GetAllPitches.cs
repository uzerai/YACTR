using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Pitches;

public class GetAllPitchesRequest : PaginationRequest {}

public class GetAllPitches : Endpoint<GetAllPitchesRequest, PaginatedResponse<PitchResponse>, PitchDataMapper>
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
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);

        await Send.OkAsync(pitches, cancellation: ct);
    }
}