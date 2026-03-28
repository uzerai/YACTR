using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Areas;

public class GetAllAreasRequest : PaginationRequest {
    /// <summary>
    /// Contains-matched name of the area. ie: "lo" will match "london" and "london wall"
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Areas created before the given instant.
    /// </summary>
    public Instant? CreatedBefore { get;init; }

    /// <summary>
    /// Areas created after the given instant.
    /// </summary>
    public Instant? CreatedAfter { get;init; }
};

public class GetAllAreas : Endpoint<GetAllAreasRequest, PaginatedResponse<AreaResponse>, AreaDataMapper>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<AreasEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllAreasRequest req, CancellationToken ct)
    {
        var query = AreaRepository.AllAvailable()
            .AsNoTracking();

        query = ApplyFilters(query, req);

        var result = await query.OrderBy(e => e.Id)
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);
            
        await Send.OkAsync(result, cancellation: ct);
    }

    private static IQueryable<Area> ApplyFilters(IQueryable<Area> query, GetAllAreasRequest req)
    {
        if (req.Name is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Name, "%" + req.Name + "%"));
        }

        if (req.CreatedBefore is not null)
        {
            query = query.Where(e => e.CreatedAt < req.CreatedBefore);
        }

        if (req.CreatedAfter is not null)
        {
            query = query.Where(e => e.CreatedAt > req.CreatedAfter);
        }

        return query;
    }
}