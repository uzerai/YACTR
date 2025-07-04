
using Microsoft.EntityFrameworkCore;
using NodaTime;

using YACTR.Data.Model;
using YACTR.Data;
using YACTR.Data.Repository.Interface;
using YACTR.Data.QueryExtensions;

namespace YACTR.Data.Repository;

public partial class EntityRepository<T> : BaseRepository<T>, IEntityRepository<T> where T : BaseEntity
{
    private readonly IClock _clock;

    public EntityRepository(DatabaseContext context) : base(context)
    {
        _clock = SystemClock.Instance;
    }

    public EntityRepository(DatabaseContext context, IClock clock)
        : base(context)
    {
        _clock = clock;
    }

    public override async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>()
            .AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public override async Task<bool> DeleteAsync(T entity)
    {
        var entityToDelete = await GetByIdTrackingAsync(entity.Id);

        if (entityToDelete == null)
        {
            return false;
        }

        entityToDelete.DeletedAt = _clock.GetCurrentInstant();
        await _context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .WhereAvailable()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<T?> GetByIdTrackingAsync(Guid id)
    {
        return await _context.Set<T>()
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
