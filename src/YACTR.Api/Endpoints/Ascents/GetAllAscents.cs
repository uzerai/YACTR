using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Achievement;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Ascents;

public class GetAllAscentsRequest : PaginationRequest {
    public Guid? RouteId { get; init; }
    public Guid? UserId { get; init; }
    public AscentType? Type { get; init; }
    public Instant? CreatedBefore { get; init; }
    public Instant? CreatedAfter { get; init; }
}

public class GetAllAscents : Endpoint<GetAllAscentsRequest, PaginatedResponse<AscentResponse>>
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
            .ToPaginatedResponseAsync((entity, ct) => Task.FromResult(new AscentResponse(
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
            query = query.Where(a => a.CreatedAt < req.CreatedBefore);
        }
        
        if (req.CreatedAfter is not null)
        {
            query = query.Where(a => a.CreatedAt > req.CreatedAfter);
        }
        return query;
    }
}