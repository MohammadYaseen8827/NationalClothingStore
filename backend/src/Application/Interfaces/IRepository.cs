namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get entities with pagination
    /// </summary>
    Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new entity
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing entity
    /// </summary>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
