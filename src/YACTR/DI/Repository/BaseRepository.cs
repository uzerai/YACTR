using Microsoft.EntityFrameworkCore;
using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;

namespace YACTR.DI.Repository;

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

    public virtual async Task<T> CreateAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<bool> DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await BuildReadonlyQuery()
            .ToListAsync();
    }

    public virtual async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
}