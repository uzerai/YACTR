using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Domain.Model;
using YACTR.Infrastructure.Database.QueryExtensions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Infrastructure.Database.Repository;

public class EntityRepository<T> : BaseRepository<T>, IEntityRepository<T> where T : BaseEntity
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
        await context.Set<T>()
            .AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);

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
        await context.SaveChangesAsync(ct);

        return true;
    }

    public override IQueryable<T> All()
    {
        return context.Set<T>()
            .AsNoTracking();
    }

    public virtual IQueryable<T> AllAvailable()
    {
        return context.Set<T>()
            .WhereAvailable();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Set<T>()
            .AsNoTracking()
            .WhereAvailable()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public virtual async Task<T?> GetByIdTrackingAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Set<T>()
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
