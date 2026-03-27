using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Areas;

public class GetAllAreasRequest : PaginationRequest {};

public class GetAllAreas : Endpoint<GetAllAreasRequest, PaginatedResponse<AreaResponse>, AreaDataMapper>
{
    public required IEntityRepository<Area> AreaRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<AreasEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllAreasRequest req, CancellationToken ct)
    {
        var areas = await AreaRepository.AllAvailable()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);
            
        await Send.OkAsync(areas, cancellation: ct);
    }
}