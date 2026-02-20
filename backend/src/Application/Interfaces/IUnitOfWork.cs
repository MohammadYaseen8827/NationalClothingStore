namespace NationalClothingStore.Application.Interfaces;

using System.Linq;

/// <summary>
/// Unit of Work interface for managing transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Database context (as object for EF Core compatibility)
    /// </summary>
    object Context { get; }

    /// <summary>
    /// Get repository for specified entity type
    /// </summary>
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Get DbSet for specified entity type
    /// </summary>
    IQueryable<T> Set<T>() where T : class;

    /// <summary>
    /// Save all changes asynchronously
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit transaction
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback transaction
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken);
}
