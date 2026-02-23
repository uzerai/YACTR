
using Microsoft.EntityFrameworkCore;
using NodaTime;

using YACTR.Data.Model;
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

    public override async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>()
            .AddAsync(entity);
        await _context.SaveChangesAsync(ct);

        return entity;
    }

    public override async Task<bool> DeleteAsync(T entity, CancellationToken ct = default)
    {
        var entityToDelete = await GetByIdTrackingAsync(entity.Id, ct);

        if (entityToDelete == null)
        {
            return false;
        }

        entityToDelete.DeletedAt = _clock.GetCurrentInstant();
        await _context.SaveChangesAsync(ct);

        return true;
    }

    public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<T>()
            .ToListAsync(ct);
    }

    public virtual async Task<IEnumerable<T>> GetAllAvailableAsync(CancellationToken ct = default)
    {
        return await _context.Set<T>()
            .WhereAvailable()
            .ToListAsync(ct);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .WhereAvailable()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public virtual async Task<T?> GetByIdTrackingAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<T>()
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
