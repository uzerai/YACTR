using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Interface.Repository;

namespace YACTR.Infrastructure.Database.Repository;

public class BaseRepository<T>(DatabaseContext context) : IRepository<T>
    where T : class
{
    protected readonly DatabaseContext context = context;

    public virtual IQueryable<T> BuildReadonlyQuery()
    {
        return context.Set<T>()
            .AsNoTracking()
            .AsQueryable();
    }

    public virtual IQueryable<T> BuildTrackedQuery()
    {
        return context.Set<T>()
            .AsTracking()
            .AsQueryable();
    }

    public virtual async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        context.Set<T>().Add(entity);
        await context.SaveChangesAsync(ct);

        return entity;
    }

    public virtual async Task<bool> DeleteAsync(T entity, CancellationToken ct = default)
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(ct);

        return true;
    }

    public virtual IQueryable<T> All()
    {
        return BuildReadonlyQuery();
    }

    public virtual async Task<bool> SaveAsync(CancellationToken ct = default)
    {
        return await context.SaveChangesAsync(ct) > 0;
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync(ct);

        return entity;
    }
}