using FastEndpoints;

using Microsoft.EntityFrameworkCore;

using NodaTime;

using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;
using YACTR.Domain.Model.Climbing.Topo;
using YACTR.Infrastructure.Service;

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

public record GetAllRoutesPitchResponseItem(
    Guid Id,
    string? Name,
    int PitchOrder,
    ClimbingType Type,
    string? Description,
    int? Grade,
    int? GearCount,
    int? Height
);

public record GetAllRoutesResponseItem(
    Guid Id,
    string Name,
    string? Description,
    ClimbingType Type,
    Guid SectorId,
    Instant CreatedAt,
    Instant UpdatedAt,
    int InSectorOrder,
    int? Grade = null,
    int? GearCount = null,
    int? Height = null,
    Instant? FirstAscentDate = null,
    string? FirstAscentClimberName = null,
    string? BolterName = null,
    Guid? TopoImageId = null,
    string? TopoImageUrl = null,
    Guid? TopoImageOverlayId = null,
    string? TopoImageOverlayUrl = null,
    Guid? SectorTopoImageId = null,
    string? SectorTopoImageUrl = null,
    Guid? SectorTopoImageOverlayId = null,
    string? SectorTopoImageOverlayUrl = null,
    GetAllRoutesPitchResponseItem[]? Pitches = default,
    TopoLinePoint[]? TopoLinePoints = default,
    TopoLinePoint[]? SectorTopoLinePoints = default
);

public class GetAllRoutes : Endpoint<GetAllRoutesRequest, PaginatedResponse<GetAllRoutesResponseItem>>
{
    public required IEntityRepository<Route> RouteRepository { get; init; }
    public required IImageStorageService ImageStorageService { get; init; }

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

        await Send.OkAsync(await result.ToPaginatedResponseAsync(MapRouteToResponseAsync, req, ct), cancellation: ct);
    }

    private async Task<GetAllRoutesResponseItem> MapRouteToResponseAsync(Route route, CancellationToken ct)
    {
        return new GetAllRoutesResponseItem(
            route.Id,
            route.Name,
            route.Description,
            route.Type,
            route.SectorId,
            route.CreatedAt,
            route.UpdatedAt,
            route.InSectorOrder,
            route.Grade,
            route.Pitches.Sum(p => p.GearCount),
            route.Pitches.Sum(p => p.Height),
            route.FirstAscentDate,
            route.FirstAscentClimberName,
            route.BolterName,
            route.TopoImageId,
            route.TopoImageId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.TopoImageId.Value, ct) : null,
            route.TopoImageOverlaySvgId,
            route.TopoImageOverlaySvgId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.TopoImageOverlaySvgId.Value, ct) : null,
            route.SectorTopoImageId,
            route.SectorTopoImageId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.SectorTopoImageId.Value, ct) : null,
            route.SectorTopoImageOverlaySvgId,
            route.SectorTopoImageOverlaySvgId.HasValue ? await ImageStorageService.GetImageUrlAsync(route.SectorTopoImageOverlaySvgId.Value, ct) : null,
            route.Pitches.Select(p => new GetAllRoutesPitchResponseItem(p.Id, p.Name, p.PitchOrder, p.Type, p.Description, p.Grade, p.GearCount, p.Height)).ToArray(),
            route.TopoLinePoints?.ToArray(),
            route.SectorTopoLinePoints?.ToArray()
        );
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