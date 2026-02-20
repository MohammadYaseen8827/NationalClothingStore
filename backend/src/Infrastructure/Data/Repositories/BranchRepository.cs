using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Branch entity
/// </summary>
public class BranchRepository(NationalClothingStoreDbContext context) : Repository<Branch>(context), IBranchRepository
{
    /// <summary>
    /// Get branch by ID with related entities
    /// </summary>
    public new async Task<Branch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Branches
            .Include(b => b.UserBranches)
            .Include(b => b.Inventories)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get branch by code
    /// </summary>
    public async Task<Branch?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Branches
            .Include(b => b.UserBranches)
            .Include(b => b.Inventories)
            .FirstOrDefaultAsync(b => b.Code == code, cancellationToken);
    }

    /// <summary>
    /// Get all active branches
    /// </summary>
    public async Task<IEnumerable<Branch>> GetActiveBranchesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Branches
            .Include(b => b.UserBranches)
            .Include(b => b.Inventories)
            .Where(b => b.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if branch code exists
    /// </summary>
    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Branches
            .AnyAsync(b => b.Code == code, cancellationToken);
    }
}
