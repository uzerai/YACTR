namespace YACTR.Api.Pagination;

public class PaginationRequest
{
    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 100;
}