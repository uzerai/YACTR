using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Sectors;

public class GetAllSectorsRequest : PaginationRequest {
    /// <summary>
    /// Contains-matched name of the area. ie: "be" will match "bearing" and "beaver wall"
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Contains-matched name of the area. ie: "lo" will match "london" and "london wall"
    /// </summary>
    public string? AreaName { get; init; }

    /// <summary>
    /// Area ID to filter by.
    /// </summary>
    public Guid? AreaId { get; init; }

    /// <summary>
    /// Sectors created before the given instant.
    /// </summary>
    public Instant? CreatedBefore { get;init; }

    /// <summary>
    /// Sectors created after the given instant.
    /// </summary>
    public Instant? CreatedAfter { get;init; }
}

public class GetAllSectors : Endpoint<GetAllSectorsRequest, PaginatedResponse<SectorResponse>, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<SectorsEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllSectorsRequest req, CancellationToken ct)
    {
        var query = SectorRepository.AllAvailable()
            .Include(e => e.Area)
            .Include(e => e.SectorImages)
            .AsNoTracking();

        query = ApplyFilters(query, req);

        var result = await query.OrderBy(e => e.Id)
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);

        await Send.OkAsync(result, cancellation: ct);
    }

    private static IQueryable<Sector> ApplyFilters(IQueryable<Sector> query, GetAllSectorsRequest req)
    {
        if (req.Name is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Name, "%" + req.Name + "%"));
        }

        if (req.AreaName is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Area.Name, "%" + req.AreaName + "%"));
        }
        
        if (req.AreaId is not null)
        {
            query = query.Where(e => e.AreaId == req.AreaId);
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