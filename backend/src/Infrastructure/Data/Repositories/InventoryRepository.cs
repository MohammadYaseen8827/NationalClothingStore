using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Inventory entity
/// </summary>
public class InventoryRepository(NationalClothingStoreDbContext context)
    : Repository<Inventory>(context), IInventoryRepository
{
    /// <summary>
    /// Get inventory by ID with related entities
    /// </summary>
    public new async Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get inventory by product and location
    /// </summary>
    public async Task<Inventory?> GetByProductAndLocationAsync(
        Guid productId, 
        Guid? productVariationId, 
        Guid branchId, 
        Guid? warehouseId, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => 
                i.ProductId == productId &&
                i.ProductVariationId == productVariationId &&
                i.BranchId == branchId &&
                i.WarehouseId == warehouseId, 
                cancellationToken);
    }

    /// <summary>
    /// Get all inventory for a specific branch
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Where(i => i.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get all inventory for a specific warehouse
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Warehouse)
            .Where(i => i.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get inventory for a specific product across all locations
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .Where(i => i.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get inventory for a specific product variation across all locations
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetByProductVariationAsync(Guid productVariationId, CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .Where(i => i.ProductVariationId == productVariationId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get low stock inventory items
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold, Guid? branchId = null , Guid? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .Where(i => i.AvailableQuantity <= threshold);
            
            if(branchId is not null)
                query = query.Where(x=> x.BranchId == branchId);
            if(warehouseId is not null)
                query = query.Where(x=> x.WarehouseId == warehouseId);      
            return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get out of stock inventory items
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .Where(i => i.AvailableQuantity == 0)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get inventory with pagination and filtering
    /// </summary>
    public async Task<(IEnumerable<Inventory> items, int totalCount)> GetWithPaginationAsync(
        int page,
        int pageSize,
        Guid? branchId = null,
        Guid? warehouseId = null,
        Guid? productId = null,
        Guid? productVariationId = null,
        bool? lowStock = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(i => i.BranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(i => i.WarehouseId == warehouseId.Value);

        if (productId.HasValue)
            query = query.Where(i => i.ProductId == productId.Value);

        if (productVariationId.HasValue)
            query = query.Where(i => i.ProductVariationId == productVariationId.Value);

        if (lowStock.HasValue)
        {
            query = lowStock.Value ? query.Where(i => i.AvailableQuantity <= 10) : query.Where(i => i.AvailableQuantity > 10);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(i => i.Product.Name)
            .ThenBy(i => i.ProductVariation.Size)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Search inventory by product name, SKU, or barcode
    /// </summary>
    public async Task<IEnumerable<Inventory>> SearchAsync(
        string searchTerm,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Inventories
            .Include(i => i.Product)
            .Include(i => i.ProductVariation)
            .Include(i => i.Branch)
            .Include(i => i.Warehouse)
            .Where(i => 
                EF.Functions.Like(i.Product.Name, $"%{searchTerm}%") ||
                EF.Functions.Like(i.Product.SKU, $"%{searchTerm}%") ||
                (i.Product.Barcode != null && EF.Functions.Like(i.Product.Barcode, $"%{searchTerm}%")))
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(i => i.BranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(i => i.WarehouseId == warehouseId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Update inventory quantity
    /// </summary>
    public async Task UpdateQuantityAsync(Guid id, int quantity, decimal unitCost, string? reason = null, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var inventory = await GetByIdAsync(id, cancellationToken);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventory with ID {id} not found");

        inventory.Quantity = quantity;
        inventory.AvailableQuantity = quantity - inventory.ReservedQuantity;
        inventory.UnitCost = unitCost;
        inventory.LastUpdated = DateTime.UtcNow;

        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Reserve inventory quantity
    /// </summary>
    public async Task ReserveQuantityAsync(Guid id, int quantity, CancellationToken cancellationToken = default)
    {
        var inventory = await GetByIdAsync(id, cancellationToken);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventory with ID {id} not found");

        if (inventory.AvailableQuantity < quantity)
            throw new InvalidOperationException($"Insufficient available quantity. Available: {inventory.AvailableQuantity}, Requested: {quantity}");

        inventory.ReservedQuantity += quantity;
        inventory.AvailableQuantity = inventory.Quantity - inventory.ReservedQuantity;
        inventory.LastUpdated = DateTime.UtcNow;

        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Release reserved inventory quantity
    /// </summary>
    public async Task ReleaseReservedQuantityAsync(Guid id, int quantity, CancellationToken cancellationToken = default)
    {
        var inventory = await GetByIdAsync(id, cancellationToken);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventory with ID {id} not found");

        if (inventory.ReservedQuantity < quantity)
            throw new InvalidOperationException($"Cannot release more than reserved quantity. Reserved: {inventory.ReservedQuantity}, Requested: {quantity}");

        inventory.ReservedQuantity -= quantity;
        inventory.AvailableQuantity = inventory.Quantity - inventory.ReservedQuantity;
        inventory.LastUpdated = DateTime.UtcNow;

        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Get inventory statistics
    /// </summary>
    public async Task<InventoryStatistics> GetStatisticsAsync(
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Inventories.AsQueryable();

        if (branchId.HasValue)
            query = query.Where(i => i.BranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(i => i.WarehouseId == warehouseId.Value);

        var inventories = await query.ToListAsync(cancellationToken);

        return new InventoryStatistics
        {
            TotalItems = inventories.Count,
            TotalQuantity = inventories.Sum(i => i.Quantity),
            TotalReservedQuantity = inventories.Sum(i => i.ReservedQuantity),
            TotalAvailableQuantity = inventories.Sum(i => i.AvailableQuantity),
            LowStockItems = inventories.Count(i => i.AvailableQuantity <= 10),
            OutOfStockItems = inventories.Count(i => i.AvailableQuantity == 0),
            TotalValue = inventories.Sum(i => i.Quantity * i.UnitCost)
        };
    }

    /// <summary>
    /// Get inventory value by location
    /// </summary>
    public async Task<IEnumerable<LocationInventoryValue>> GetInventoryValueByLocationAsync(CancellationToken cancellationToken = default)
    {
        var branchValues = await Context.Inventories
            .GroupBy(i => i.BranchId)
            .Select(g => new LocationInventoryValue
            {
                LocationId = g.Key,
                LocationType = "Branch",
                TotalValue = g.Sum(i => i.Quantity * i.UnitCost),
                TotalQuantity = g.Sum(i => i.Quantity),
                ItemCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        var warehouseValues = await Context.Inventories
            .Where(i => i.WarehouseId != null)
            .GroupBy(i => i.WarehouseId)
            .Select(g => new LocationInventoryValue
            {
                LocationId = g.Key!.Value,
                LocationType = "Warehouse",
                TotalValue = g.Sum(i => i.Quantity * i.UnitCost),
                TotalQuantity = g.Sum(i => i.Quantity),
                ItemCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        return branchValues.Concat(warehouseValues);
    }

    /// <summary>
    /// Check if inventory exists for product and location
    /// </summary>
    public async Task<bool> ExistsAsync(Guid productId, Guid? productVariationId, Guid branchId, Guid? warehouseId, CancellationToken cancellationToken = default)
    {
        return await Context.Inventories
            .AnyAsync(i => 
                i.ProductId == productId &&
                i.ProductVariationId == productVariationId &&
                i.BranchId == branchId &&
                i.WarehouseId == warehouseId, 
                cancellationToken);
    }

    /// <summary>
    /// Get inventory movements for a specific period
    /// </summary>
    public async Task<IEnumerable<InventoryMovement>> GetMovementsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryTransactions
            .Include(t => t.Inventory)
                .ThenInclude(i => i.Product)
            .Include(t => t.Inventory)
                .ThenInclude(i => i.ProductVariation)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(t => t.Inventory.BranchId == branchId.Value || t.ToBranchId == branchId.Value);

        if (warehouseId.HasValue)
            query = query.Where(t => t.Inventory.WarehouseId == warehouseId.Value || t.ToWarehouseId == warehouseId.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new InventoryMovement
            {
                TransactionId = t.Id,
                ProductId = t.Inventory.ProductId,
                ProductVariationId = t.Inventory.ProductVariationId,
                ProductName = t.Inventory.Product.Name,
                ProductSku = t.Inventory.Product.SKU,
                ProductVariationSize = t.Inventory.ProductVariation != null ? t.Inventory.ProductVariation.Size : null,
                ProductVariationColor = t.Inventory.ProductVariation != null ? t.Inventory.ProductVariation.Color : null,
                TransactionType = t.TransactionType,
                Quantity = t.Quantity,
                UnitCost = t.UnitCost,
                ReferenceNumber = t.ReferenceNumber,
                Reason = t.Reason,
                FromBranchId = t.FromBranchId,
                ToBranchId = t.ToBranchId,
                FromWarehouseId = t.FromWarehouseId,
                ToWarehouseId = t.ToWarehouseId,
                CreatedAt = t.CreatedAt,
                CreatedByUserId = t.CreatedByUserId
            })
            .ToListAsync(cancellationToken);
    }
}
