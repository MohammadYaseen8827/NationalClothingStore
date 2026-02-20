using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for PurchaseOrder entity
/// </summary>
public class PurchaseOrderRepository(NationalClothingStoreDbContext context)
    : Repository<PurchaseOrder>(context), IPurchaseOrderRepository
{
    public new async Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public new async Task<IEnumerable<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .Where(po => po.SupplierId == supplierId)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .Where(po => po.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<PurchaseOrder> orders, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        string? status = null,
        Guid? supplierId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.PurchaseOrders.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(po =>
                po.OrderNumber.Contains(search) ||
                po.Supplier.Name.Contains(search) ||
                po.Items.Any(item => item.Product.Name.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(po => po.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (supplierId.HasValue)
        {
            query = query.Where(po => po.SupplierId == supplierId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var orders = await query
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .OrderByDescending(po => po.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (orders, totalCount);
    }

    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        purchaseOrder.OrderNumber = GenerateOrderNumber();
        purchaseOrder.OrderDate = DateTime.UtcNow;
        purchaseOrder.Status = "Pending";
        purchaseOrder.CreatedAt = DateTime.UtcNow;
        purchaseOrder.UpdatedAt = DateTime.UtcNow;
        
        await Context.PurchaseOrders.AddAsync(purchaseOrder, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        
        return purchaseOrder;
    }

    public new async Task<PurchaseOrder> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        purchaseOrder.UpdatedAt = DateTime.UtcNow;
        
        Context.PurchaseOrders.Update(purchaseOrder);
        await Context.SaveChangesAsync(cancellationToken);
        
        return purchaseOrder;
    }

    public new async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var purchaseOrder = await GetByIdAsync(id, cancellationToken);
        if (purchaseOrder == null)
            return false;

        Context.PurchaseOrders.Remove(purchaseOrder);
        await Context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .AnyAsync(po => po.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders.CountAsync(cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .CountAsync(po => po.Status.Equals(status, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<decimal> GetTotalValueAsync(CancellationToken cancellationToken = default)
    {
        return await Context.PurchaseOrders
            .Where(po => po.Status != "Cancelled")
            .SumAsync(po => po.TotalAmount, cancellationToken);
    }

    private string GenerateOrderNumber()
    {
        return $"PO{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
