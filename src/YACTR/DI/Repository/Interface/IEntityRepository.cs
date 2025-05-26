using YACTR.Model;

namespace YACTR.DI.Repository.Interface;

public interface IEntityRepository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Get an entity by its ID without tracking changes.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get an entity by its ID with change tracking enabled.
    /// 
    /// Aims to be the only entrypoint to getting a 
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdTrackingAsync(Guid id);
}
