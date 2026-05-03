using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.QueryExtensions;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Endpoints.Sectors;

public class GetAllSectorsRequest : PaginationRequest
{
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
    public Instant? CreatedBefore { get; init; }

    /// <summary>
    /// Sectors created after the given instant.
    /// </summary>
    public Instant? CreatedAfter { get; init; }
}

public record GetAllSectorsImageResponse(
    Guid ImageId,
    int Order,
    string? ImageUrl
);

public record GetAllSectorsResponseItem(
    Guid Id,
    string Name,
    Polygon SectorArea,
    Point? EntryPoint,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid AreaId,
    string AreaName,
    Guid? PrimarySectorImageId,
    string? PrimarySectorImageUrl,
    IEnumerable<GetAllSectorsImageResponse> SectorImages,
    Instant CreatedAt,
    Instant UpdatedAt
);

public class GetAllSectors : Endpoint<GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }
    public required IImageStorageService ImageStorageService { get; init; }

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
            .ToPaginatedResponseAsync(MapSectorToResponseAsync, req, ct);

        await Send.OkAsync(result, cancellation: ct);
    }

    private async Task<GetAllSectorsResponseItem> MapSectorToResponseAsync(Sector sector, CancellationToken ct)
    {
        return new GetAllSectorsResponseItem(
            sector.Id,
            sector.Name,
            sector.SectorArea,
            sector.EntryPoint,
            sector.RecommendedParkingLocation,
            sector.ApproachPath,
            sector.AreaId,
            sector.Area.Name,
            sector.PrimarySectorImageId,
            sector.PrimarySectorImageId.HasValue ? await ImageStorageService.GetImageUrlAsync(sector.PrimarySectorImageId.Value, ct) : null,
            await Task.WhenAll(sector.SectorImages.Select(async sI => new GetAllSectorsImageResponse(sI.ImageId, sI.Order, await ImageStorageService.GetImageUrlAsync(sI.ImageId, ct)))),
            sector.CreatedAt,
            sector.UpdatedAt
        );
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
            query = query.WhereCreatedAtBefore(req.CreatedBefore.Value);
        }

        if (req.CreatedAfter is not null)
        {
            query = query.WhereCreatedAtAfter(req.CreatedAfter.Value);
        }

        return query;
    }
}