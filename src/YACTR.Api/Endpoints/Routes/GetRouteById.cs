using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;
using YACTR.Domain.Model.Climbing.Topo;
using YACTR.Infrastructure.Database.QueryExtensions;
using YACTR.Infrastructure.Service;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Api.Endpoints.Routes;

public record GetRouteByIdRequest(Guid RouteId);

public record GetRouteByIdPitchResponse(
    Guid Id,
    string? Name,
    int PitchOrder,
    ClimbingType Type,
    string? Description,
    int? Grade,
    int? GearCount,
    int? Height
);

public record GetRouteByIdResponse(
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
    GetRouteByIdPitchResponse[]? Pitches = default,
    TopoLinePoint[]? TopoLinePoints = default,
    TopoLinePoint[]? SectorTopoLinePoints = default
);

public class GetRouteById([Required] IEntityRepository<Route> routeRepository, [Required] IImageStorageService imageStorageService) : Endpoint<GetRouteByIdRequest, GetRouteByIdResponse>
{
    public override void Configure()
    {
        Get("/{route_id}");
        Group<RoutesEndpointGroup>();
    }

    public override async Task HandleAsync(GetRouteByIdRequest req, CancellationToken ct)
    {
        var route = await routeRepository.BuildReadonlyQuery()
            .Where(e => e.Id == req.RouteId)
            .WhereAvailable()
            .Include(e => e.Pitches.Where(x => x.DeletedAt == null))
            .Include(e => e.TopoImage)
            .Include(e => e.TopoImageOverlaySvg)
            .Include(e => e.SectorTopoImage)
            .Include(e => e.SectorTopoImageOverlaySvg)
            .FirstOrDefaultAsync(ct);

        if (route == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(await MapRouteToResponseAsync(route, ct), cancellation: ct);
    }

    private async Task<GetRouteByIdResponse> MapRouteToResponseAsync(Route route, CancellationToken ct)
    {
        return new GetRouteByIdResponse(
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
            route.TopoImageId.HasValue ? await imageStorageService.GetImageUrlAsync(route.TopoImageId.Value, ct) : null,
            route.TopoImageOverlaySvgId,
            route.TopoImageOverlaySvgId.HasValue ? await imageStorageService.GetImageUrlAsync(route.TopoImageOverlaySvgId.Value, ct) : null,
            route.SectorTopoImageId,
            route.SectorTopoImageId.HasValue ? await imageStorageService.GetImageUrlAsync(route.SectorTopoImageId.Value, ct) : null,
            route.SectorTopoImageOverlaySvgId,
            route.SectorTopoImageOverlaySvgId.HasValue ? await imageStorageService.GetImageUrlAsync(route.SectorTopoImageOverlaySvgId.Value, ct) : null,
            route.Pitches.Select(p => new GetRouteByIdPitchResponse(p.Id, p.Name, p.PitchOrder, p.Type, p.Description, p.Grade, p.GearCount, p.Height)).ToArray(),
            route.TopoLinePoints?.ToArray(),
            route.SectorTopoLinePoints?.ToArray()
        );
    }
}