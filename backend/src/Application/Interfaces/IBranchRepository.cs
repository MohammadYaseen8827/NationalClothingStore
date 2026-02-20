using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for Branch entity
/// </summary>
public interface IBranchRepository : IRepository<Branch>
{
    /// <summary>
    /// Get branch by ID with related entities
    /// </summary>
    new Task<Branch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get branch by code
    /// </summary>
    Task<Branch?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active branches
    /// </summary>
    Task<IEnumerable<Branch>> GetActiveBranchesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if branch code exists
    /// </summary>
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
