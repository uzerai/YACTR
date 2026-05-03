using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.QueryExtensions;

namespace YACTR.Api.Endpoints.Areas;

public class GetAllAreasRequest : PaginationRequest
{
    /// <summary>
    /// Contains-matched name of the area. ie: "lo" will match "london" and "london wall"
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Areas created before the given instant.
    /// </summary>
    public Instant? CreatedBefore { get; init; }

    /// <summary>
    /// Areas created after the given instant.
    /// </summary>
    public Instant? CreatedAfter { get; init; }

    /// <summary>
    /// Country name to filter by.
    /// </summary>
    public string? CountryName { get; init; }

    /// <summary>
    /// Country ID to filter by.
    /// </summary>
    public int? CountryId { get; init; }
};

public record GetAllAreasCountryResponseItem(
    int Id,
    string Name,
    string Code
);

public record GetAllAreasResponseItem(
    Guid Id,
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary,
    Instant CreatedAt,
    Instant UpdatedAt,
    GetAllAreasCountryResponseItem Country
);

public class GetAllAreas : Endpoint<GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>
{
    public required IEntityRepository<Area> AreaRepository { get; init;  }

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

        var result = await query
            .Include(x => x.Country)
            .OrderBy(e => e.Id)
            .ToPaginatedResponseAsync(MapAreaToResponseAsync, req, ct);

        await Send.OkAsync(result, cancellation: ct);
    }

    private static Task<GetAllAreasResponseItem> MapAreaToResponseAsync(Area area, CancellationToken ct)
    {
        return Task.FromResult(new GetAllAreasResponseItem(
            Id: area.Id,
            Name: area.Name,
            Description: area.Description,
            Location: area.Location,
            Boundary: area.Boundary,
            CreatedAt: area.CreatedAt,
            UpdatedAt: area.UpdatedAt,
            Country: new GetAllAreasCountryResponseItem(area.CountryId, area.Country.AdminName, area.Country.Code)
        ));
    }

    private static IQueryable<Area> ApplyFilters(IQueryable<Area> query, GetAllAreasRequest req)
    {
        if (req.Name is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Name, "%" + req.Name + "%"));
        }

        if (req.CreatedBefore is not null)
        {
            query = query.WhereCreatedAtBefore(req.CreatedBefore.Value);
        }

        if (req.CreatedAfter is not null)
        {
            query = query.WhereCreatedAtAfter(req.CreatedAfter.Value);
        }

        if (req.CountryName is not null)
        {
            query = query.Where(e => e.Country.CountryName == req.CountryName);
        }

        if (req.CountryId.HasValue)
        {
            query = query.Where(e => e.CountryId == req.CountryId.Value);
        }

        return query;
    }
}