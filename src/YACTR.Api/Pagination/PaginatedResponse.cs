namespace YACTR.Api.Pagination;

public class PaginatedResponse<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
}