using Microsoft.EntityFrameworkCore;

namespace YACTR.Api.Pagination;

public static class ToPaginatedResponseQueryExtension
{
    private const int MaxPageSize = 500;
    private const int DefaultPageSize = 100;
    private const int MinPageSize = 1;

    private static int GetTake(PaginationRequest paginationRequest)
    {
        return Math.Clamp(paginationRequest.PageSize ?? DefaultPageSize, MinPageSize, MaxPageSize);
    }

    private static int GetSkip(PaginationRequest paginationRequest)
    {
        return Math.Clamp((paginationRequest.Page ?? 1) - 1, 0, int.MaxValue) * GetTake(paginationRequest);
    }

    public static PaginatedResponse<TResponseData> ToPaginatedResponse<TPaginationRequest, TResponseData, TEntity>(
        this IQueryable<TEntity> query, 
        Func<TEntity, TResponseData> entityToResponseMapper, 
        TPaginationRequest paginationRequest) 
            where TPaginationRequest : PaginationRequest
            where TResponseData : class
            where TEntity : class
    {
        return new PaginatedResponse<TResponseData> {
            Items = query.Skip(GetSkip(paginationRequest))
                .Take(GetTake(paginationRequest))
                .Select(entityToResponseMapper)
                .ToList(),
            TotalCount = query.Count()
        };
    }

    public static async Task<PaginatedResponse<TResponseData>> ToPaginatedResponseAsync<TPaginationRequest, TResponseData, TEntity>(
        this IQueryable<TEntity> query,
        Func<TEntity, CancellationToken, Task<TResponseData>> entityToResponseMapper,
        TPaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
            where TPaginationRequest : PaginationRequest
            where TResponseData : class
            where TEntity : class
    {
        var pagedEntities = await query.Skip(GetSkip(paginationRequest))
            .Take(GetTake(paginationRequest))
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TResponseData> {
            Items = (await Task.WhenAll(
                pagedEntities.Select(entity => entityToResponseMapper(entity, cancellationToken))
            )).ToList(),
            TotalCount = await query.CountAsync(cancellationToken)
        };
    }
}