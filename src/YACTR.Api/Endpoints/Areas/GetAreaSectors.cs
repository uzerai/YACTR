using FastEndpoints;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;

using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Endpoints.Areas;

public record GetAreaSectorsRequest(Guid AreaId);

public record GetAreaSectorsImageResponse(
    Guid ImageId,
    int Order,
    string? ImageUrl
);

public record GetAreaSectorsResponseItem(
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
    IEnumerable<GetAreaSectorsImageResponse> SectorImages,
    Instant CreatedAt,
    Instant UpdatedAt
);

public class GetAreaSectors : Endpoint<GetAreaSectorsRequest, IEnumerable<GetAreaSectorsResponseItem>>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }
    public required IEntityRepository<Area> AreaRepository { get; init; }
    public required IImageStorageService ImageStorageService { get; init; }

    public override void Configure()
    {
        Get("/{area_id}/sectors");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(GetAreaSectorsRequest req, CancellationToken ct)
    {
        var areaExists = await AreaRepository.BuildReadonlyQuery().AnyAsync(e => e.Id == req.AreaId, ct);
        if (!areaExists)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var sectors = await SectorRepository.AllAvailable()
            .Where(e => e.AreaId == req.AreaId)
            .Include(e => e.Area)
            .Include(e => e.SectorImages)
            .ToListAsync(ct);

        await Send.OkAsync(await Task.WhenAll(sectors.Select(e => MapSectorToResponseAsync(e, ct))), cancellation: ct);
    }

    private async Task<GetAreaSectorsResponseItem> MapSectorToResponseAsync(Sector sector, CancellationToken ct)
    {
        return new GetAreaSectorsResponseItem(
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
            await Task.WhenAll(sector.SectorImages.Select(async sI => new GetAreaSectorsImageResponse(sI.ImageId, sI.Order, await ImageStorageService.GetImageUrlAsync(sI.ImageId, ct)))),
            sector.CreatedAt,
            sector.UpdatedAt
        );
    }
}