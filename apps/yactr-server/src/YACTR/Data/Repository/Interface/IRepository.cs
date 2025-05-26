namespace YACTR.Data.Repository.Interface;

public interface IRepository<T>
{
    /// <summary>
    /// Build a queryable collection of entities.
    /// 
    /// Forces tracking of the EF entity.
    /// </summary>
    /// <returns>A queryable collection of entities.</returns>
    IQueryable<T> BuildTrackedQuery();

    /// <summary>
    /// Build a queryable collection of entities.
    /// 
    /// Forces no tracking of the EF entity.
    /// </summary>
    /// <returns>A queryable collection of entities.</returns>
    IQueryable<T> BuildReadonlyQuery();

    /// <summary>
    /// Get all entities of type T.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Create a new entity in the database.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity with updated properties (e.g., ID, timestamps).</returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Update an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Soft delete an entity by setting its DeletedAt property.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>True if the entity was found and deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(T entity);

    /// <summary>
    /// Save changes made to the database context.
    /// </summary>
    /// <returns>True if any changes were saved; otherwise, false.</returns>
    Task<bool> SaveAsync();
}
