using Microsoft.EntityFrameworkCore;
using YACTR.Data.Repository.Interface;

namespace YACTR.Data.Repository;

public partial class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly DatabaseContext _context;

    public BaseRepository(DatabaseContext context)
    {
        _context = context;
    }

    public virtual IQueryable<T> BuildReadonlyQuery()
    {
        return _context.Set<T>()
            .AsNoTracking()
            .AsQueryable();
    }

    public virtual IQueryable<T> BuildTrackedQuery()
    {
        return _context.Set<T>()
            .AsTracking()
            .AsQueryable();
    }

    public virtual async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync(ct);

        return entity;
    }

    public virtual async Task<bool> DeleteAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(ct);

        return true;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await BuildReadonlyQuery()
            .ToListAsync(ct);
    }

    public virtual async Task<bool> SaveAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(ct);

        return entity;
    }
}