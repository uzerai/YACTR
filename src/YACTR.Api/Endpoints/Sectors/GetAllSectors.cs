using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Sectors;

public class GetAllSectorsRequest : PaginationRequest {}

public class GetAllSectors : Endpoint<GetAllSectorsRequest, PaginatedResponse<SectorResponse>, SectorDataMapper>
{
    public required IEntityRepository<Sector> SectorRepository { get; init; }

    public override void Configure()
    {
        Get("/");
        Group<SectorsEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllSectorsRequest req, CancellationToken ct)
    {
        var sectors = await SectorRepository.AllAvailable()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToPaginatedResponseAsync(Map.FromEntityAsync, req, ct);

        await Send.OkAsync(sectors, cancellation: ct);
    }
}