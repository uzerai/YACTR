using YACTR.Domain.Model;

namespace YACTR.Domain.Interface.Repository;

public interface IEntityRepository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Get an entity by its ID without tracking changes.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Get an entity by its ID with change tracking enabled.
    /// 
    /// Aims to be the only entrypoint to getting a 
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdTrackingAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Get all available (where deleted at is null) entities.
    /// </summary>
    /// <returns>A collection of all available entities.</returns>
    IQueryable<T> AllAvailable();
}
