using NodaTime;
using YACTR.Model;

namespace YACTR.DI.Data.QueryExtensions;

public static class BaseEntityQueryExtensions
{
    public static IQueryable<T> WhereAvailable<T>(this IQueryable<T> query) where T : BaseEntity => query.Where(x => x.DeletedAt == null);
    public static IQueryable<T> WhereDeleted<T>(this IQueryable<T> query) where T : BaseEntity => query.Where(x => x.DeletedAt != null);
    public static IQueryable<T> WhereCreatedAtBefore<T>(this IQueryable<T> query, Instant instant) where T : BaseEntity => query.Where(x => x.CreatedAt < instant);
    public static IQueryable<T> WhereCreatedAtAfter<T>(this IQueryable<T> query, Instant instant) where T : BaseEntity => query.Where(x => x.CreatedAt > instant);
    public static IQueryable<T> WhereCreatedAtBetween<T>(this IQueryable<T> query, Instant start, Instant end) where T : BaseEntity => query.Where(x => x.CreatedAt >= start && x.CreatedAt <= end);

}
