using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Sectors;

public class UpdateSectorRequest
{
    public Guid SectorId { get; set; }

    [FromBody]
    public required SectorRequestData Data { get; set; }

}

public class UpdateSector : AuthenticatedEndpoint<UpdateSectorRequest, EmptyResponse>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Put("/{SectorId}");
        Group<SectorsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.SectorsWrite)));
    }

    public override async Task HandleAsync(UpdateSectorRequest req, CancellationToken ct)
    {
        var existingSector = await SectorRepository.GetByIdAsync(req.SectorId, ct);
        if (existingSector == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        existingSector.Name = req.Data.Name;
        existingSector.SectorArea = req.Data.SectorArea;
        existingSector.EntryPoint = req.Data.EntryPoint;
        existingSector.RecommendedParkingLocation = req.Data.RecommendedParkingLocation;
        existingSector.ApproachPath = req.Data.ApproachPath;
        existingSector.AreaId = req.Data.AreaId;

        await SectorRepository.UpdateAsync(existingSector, ct);

        await SendNoContentAsync(ct);
    }
}