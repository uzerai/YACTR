using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.QueryExtensions;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Endpoints.Sectors;

public record GetSectorByIdRequest(Guid SectorId);

public record GetSectorByIdImageResponse(
    Guid ImageId,
    int Order,
    string? ImageUrl
);

public record GetSectorByIdResponse(
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
    IEnumerable<GetSectorByIdImageResponse> SectorImages,
    Instant CreatedAt,
    Instant UpdatedAt
);

public class GetSectorById : Endpoint<GetSectorByIdRequest, GetSectorByIdResponse>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }
    public required IImageStorageService ImageStorageService { get; init; }

    public override void Configure()
    {
        Get("/{sector_id}");
        Group<SectorsEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSectorByIdRequest req, CancellationToken ct)
    {
        var sector = await SectorRepository
            .BuildReadonlyQuery()
            .WhereAvailable()
            .Where(e => e.Id == req.SectorId)
            .Include(e => e.Area)
            .Include("SectorImages")
            .FirstOrDefaultAsync(ct);

        if (sector == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(await MapSectorToResponseAsync(sector, ct), cancellation: ct);
    }

    private async Task<GetSectorByIdResponse> MapSectorToResponseAsync(Sector sector, CancellationToken ct)
    {
        return new GetSectorByIdResponse(
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
            await Task.WhenAll(sector.SectorImages.Select(async sI => new GetSectorByIdImageResponse(sI.ImageId, sI.Order, await ImageStorageService.GetImageUrlAsync(sI.ImageId, ct)))),
            sector.CreatedAt,
            sector.UpdatedAt
        );
    }
}