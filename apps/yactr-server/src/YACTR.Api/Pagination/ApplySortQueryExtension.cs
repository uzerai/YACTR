using System.Linq.Expressions;

namespace YACTR.Api.Pagination;

public static class ApplySortQueryExtension
{
    /// <summary>
    /// Orders the query according to the request's <c>sort_by</c>/<c>direction</c> parameters,
    /// using the given allowlist of key selectors. A tiebreaker (typically the entity id) is always
    /// appended so paging is deterministic.
    /// </summary>
    public static IOrderedQueryable<TEntity> ApplySort<TEntity, TSortBy>(
        this IQueryable<TEntity> query,
        SortedPaginationRequest<TSortBy> request,
        IReadOnlyDictionary<TSortBy, Expression<Func<TEntity, object>>> keySelectors,
        TSortBy defaultSortBy,
        SortDirection defaultDirection,
        Expression<Func<TEntity, object>> tiebreaker)
        where TSortBy : struct, Enum
    {
        var sortBy = request.SortBy ?? defaultSortBy;
        var direction = request.Direction ?? defaultDirection;
        var keySelector = keySelectors[sortBy];

        var ordered = direction == SortDirection.Desc
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);

        return ordered.ThenBy(tiebreaker);
    }
}
