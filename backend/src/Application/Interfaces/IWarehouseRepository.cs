using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for Warehouse entity
/// </summary>
public interface IWarehouseRepository : IRepository<Warehouse>
{
    /// <summary>
    /// Get warehouse by ID with related entities
    /// </summary>
    new Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get warehouse by code
    /// </summary>
    Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active warehouses
    /// </summary>
    Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if warehouse code exists
    /// </summary>
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
