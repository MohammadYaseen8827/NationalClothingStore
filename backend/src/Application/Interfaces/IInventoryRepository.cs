using NationalClothingStore.Domain.Entities;
using NationalClothingStore.Application.Common;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for Inventory entity
/// </summary>
public interface IInventoryRepository : IRepository<Inventory>
{
    /// <summary>
    /// Get inventory by ID with related entities
    /// </summary>
    new Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory by product and location
    /// </summary>
    Task<Inventory?> GetByProductAndLocationAsync(
        Guid productId, 
        Guid? productVariationId, 
        Guid branchId, 
        Guid? warehouseId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all inventory for a specific branch
    /// </summary>
    Task<IEnumerable<Inventory>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all inventory for a specific warehouse
    /// </summary>
    Task<IEnumerable<Inventory>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory for a specific product across all locations
    /// </summary>
    Task<IEnumerable<Inventory>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory for a specific product variation across all locations
    /// </summary>
    Task<IEnumerable<Inventory>> GetByProductVariationAsync(Guid productVariationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get low stock inventory items
    /// </summary>
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold, Guid? branchId = null , Guid? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get out of stock inventory items
    /// </summary>
    Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory with pagination and filtering
    /// </summary>
    Task<(IEnumerable<Inventory> items, int totalCount)> GetWithPaginationAsync(
        int page,
        int pageSize,
        Guid? branchId = null,
        Guid? warehouseId = null,
        Guid? productId = null,
        Guid? productVariationId = null,
        bool? lowStock = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search inventory by product name, SKU, or barcode
    /// </summary>
    Task<IEnumerable<Inventory>> SearchAsync(
        string searchTerm,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update inventory quantity
    /// </summary>
    Task UpdateQuantityAsync(Guid id, int quantity, decimal unitCost, string? reason = null, Guid? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reserve inventory quantity
    /// </summary>
    Task ReserveQuantityAsync(Guid id, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Release reserved inventory quantity
    /// </summary>
    Task ReleaseReservedQuantityAsync(Guid id, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory statistics
    /// </summary>
    Task<InventoryStatistics> GetStatisticsAsync(
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory value by location
    /// </summary>
    Task<IEnumerable<LocationInventoryValue>> GetInventoryValueByLocationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if inventory exists for product and location
    /// </summary>
    Task<bool> ExistsAsync(Guid productId, Guid? productVariationId, Guid branchId, Guid? warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory movements for a specific period
    /// </summary>
    Task<IEnumerable<InventoryMovement>> GetMovementsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
}
