namespace YACTR.Api.Pagination;

/// <summary>
/// Pagination request with typed sorting. <typeparamref name="TSortBy"/> is a per-endpoint enum
/// acting as an allowlist of sortable columns; it binds from the <c>sort_by</c> query parameter
/// and <see cref="Direction"/> binds from <c>direction</c>.
/// </summary>
public abstract class SortedPaginationRequest<TSortBy> : PaginationRequest
    where TSortBy : struct, Enum
{
    public TSortBy? SortBy { get; init; }
    public SortDirection? Direction { get; init; }
}
