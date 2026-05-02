using FastEndpoints;
using NetTopologySuite.Geometries;
using NodaTime;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Areas;

public record GetAreaByIdRequest(Guid AreaId);

public record GetAreaByIdResponse(
    Guid Id,
    string Name,
    string? Description,
    Point Location,
    MultiPolygon Boundary,
    Instant CreatedAt,
    Instant UpdatedAt
);

public class GetAreaById : Endpoint<GetAreaByIdRequest, GetAreaByIdResponse>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Get("/{area_id}");
        Group<AreasEndpointGroup>();
    }

    public override async Task HandleAsync(GetAreaByIdRequest req, CancellationToken ct)
    {
        var area = await AreaRepository.GetByIdAsync(req.AreaId, ct);

        if (area == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new GetAreaByIdResponse(
            Id: area.Id,
            Name: area.Name,
            Description: area.Description,
            Location: area.Location,
            Boundary: area.Boundary,
            CreatedAt: area.CreatedAt,
            UpdatedAt: area.UpdatedAt
        ), cancellation: ct);
    }
}