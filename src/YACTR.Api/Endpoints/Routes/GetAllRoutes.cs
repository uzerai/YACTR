using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using NodaTime;

using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;

using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public class GetAllRoutesRequest : PaginationRequest
{
    public string? Name { get; init; }
    public string? SectorName { get; init; }
    public Guid? SectorId { get; init; }
    public string? AreaName { get; init; }
    public Guid? AreaId { get; init; }
    public string? CountryName { get; init; }
    public int? CountryId { get; init; }
    public ClimbingType? Type { get; init; }
    public Instant? CreatedBefore { get; init; }
    public Instant? CreatedAfter { get; init; }
}

public class GetAllRoutes : Endpoint<GetAllRoutesRequest, PaginatedResponse<RouteResponse>, RouteDataMapper>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetAllRoutesRequest req, CancellationToken ct)
    {
        var query = RouteRepository.AllAvailable()
            .AsNoTracking();

        var result = ApplyFilters(query, req);

        await Send.OkAsync(await result.ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct), cancellation: ct);
    }

    private static IQueryable<Route> ApplyFilters(IQueryable<Route> query, GetAllRoutesRequest req)
    {
        if (req.Name is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Name, "%" + req.Name + "%"));
        }

        if (req.SectorName is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Sector.Name, "%" + req.SectorName + "%"));
        }

        if (req.SectorId is not null)
        {
            query = query.Where(e => e.SectorId == req.SectorId);
        }

        if (req.AreaName is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Sector.Area.Name, "%" + req.AreaName + "%"));
        }

        if (req.AreaId is not null)
        {
            query = query.Where(e => e.Sector.AreaId == req.AreaId);
        }

        if (req.Type is not null)
        {
            query = query.Where(e => e.Type == req.Type);
        }

        if (req.CreatedBefore is not null)
        {
            query = query.Where(e => e.CreatedAt < req.CreatedBefore);
        }

        if (req.CreatedAfter is not null)
        {
            query = query.Where(e => e.CreatedAt > req.CreatedAfter);
        }

        if (req.CountryName is not null)
        {
            query = query.Where(e => EF.Functions.ILike(e.Sector.Area.Country.CountryName, "%" + req.CountryName + "%"));
        }

        if (req.CountryId.HasValue)
        {
            query = query.Where(e => e.Sector.Area.CountryId == req.CountryId.Value);
        }

        return query;
    }
}