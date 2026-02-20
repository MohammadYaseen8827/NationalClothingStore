using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Warehouse entity
/// </summary>
public class WarehouseRepository(NationalClothingStoreDbContext context)
    : Repository<Warehouse>(context), IWarehouseRepository
{
    /// <summary>
    /// Get warehouse by ID with related entities
    /// </summary>
    public new async Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Warehouses
            .Include(w => w.Inventories)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get warehouse by code
    /// </summary>
    public async Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Warehouses
            .Include(w => w.Inventories)
            .FirstOrDefaultAsync(w => w.Code == code, cancellationToken);
    }

    /// <summary>
    /// Get all active warehouses
    /// </summary>
    public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Warehouses
            .Include(w => w.Inventories)
            .Where(w => w.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if warehouse code exists
    /// </summary>
    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Warehouses
            .AnyAsync(w => w.Code == code, cancellationToken);
    }
}
