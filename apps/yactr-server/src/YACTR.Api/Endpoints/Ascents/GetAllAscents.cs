using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Achievement;
using YACTR.Infrastructure.Database.QueryExtensions;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Ascents;

public class GetAllAscentsRequest : PaginationRequest
{
    public Guid? RouteId { get; init; }
    public Guid? UserId { get; init; }
    public AscentType? Type { get; init; }
    public Instant? CreatedBefore { get; init; }
    public Instant? CreatedAfter { get; init; }
}

public record GetAllAscentsResponseItem(
  Guid Id,
  Guid UserId,
  AscentType Type,
  Instant CompletedAt,
  Route? Route
);

public class GetAllAscents : Endpoint<GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>
{
    public required IRepository<Ascent> AscentRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<AscentsEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllAscentsRequest req, CancellationToken ct)
    {
        var query = AscentRepository.BuildReadonlyQuery()
            .Include(a => a.Route)
            .AsQueryable();

        query = ApplyFilters(query, req);

        var result = await query.OrderByDescending(a => a.CompletedAt)
            .ToPaginatedResponseAsync((entity, ct) => Task.FromResult(new GetAllAscentsResponseItem(
                entity.Id, entity.UserId, entity.Type, entity.CompletedAt, entity.Route)), req, ct);

        await Send.OkAsync(result, cancellation: ct);
    }

    private static IQueryable<Ascent> ApplyFilters(IQueryable<Ascent> query, GetAllAscentsRequest req)
    {
        if (req.RouteId is not null)
        {
            query = query.Where(a => a.RouteId == req.RouteId);
        }

        if (req.UserId is not null)
        {
            query = query.Where(a => a.UserId == req.UserId);
        }

        if (req.Type is not null)
        {
            query = query.Where(a => a.Type == req.Type);
        }

        if (req.CreatedBefore is not null)
        {
            query = query.WhereCreatedAtBefore(req.CreatedBefore.Value);
        }

        if (req.CreatedAfter is not null)
        {
            query = query.WhereCreatedAtAfter(req.CreatedAfter.Value);
        }
        return query;
    }
}