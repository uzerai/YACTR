using YACTR.Data.Model;

namespace YACTR.Data.Repository.Interface;

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
}
