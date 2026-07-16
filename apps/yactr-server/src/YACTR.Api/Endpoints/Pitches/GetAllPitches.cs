using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Pitches;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GetAllPitchesSortBy
{
    [EnumMember(Value = "name")]
    [JsonStringEnumMemberName("name")]
    Name,

    [EnumMember(Value = "grade")]
    [JsonStringEnumMemberName("grade")]
    Grade,

    [EnumMember(Value = "pitch_order")]
    [JsonStringEnumMemberName("pitch_order")]
    PitchOrder,

    [EnumMember(Value = "created_at")]
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,
}

public class GetAllPitchesRequest : SortedPaginationRequest<GetAllPitchesSortBy> { }

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
    private static readonly IReadOnlyDictionary<GetAllPitchesSortBy, Expression<Func<Pitch, object>>> SortKeySelectors =
        new Dictionary<GetAllPitchesSortBy, Expression<Func<Pitch, object>>>
        {
            [GetAllPitchesSortBy.Name] = e => e.Name!,
            [GetAllPitchesSortBy.Grade] = e => e.Grade,
            [GetAllPitchesSortBy.PitchOrder] = e => e.PitchOrder,
            [GetAllPitchesSortBy.CreatedAt] = e => e.CreatedAt,
        };

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
            .ApplySort(req, SortKeySelectors, GetAllPitchesSortBy.CreatedAt, SortDirection.Desc, e => e.Id)
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