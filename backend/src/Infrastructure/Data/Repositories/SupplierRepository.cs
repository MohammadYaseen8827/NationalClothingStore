using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Supplier entity
/// </summary>
public class SupplierRepository(NationalClothingStoreDbContext context)
    : Repository<Supplier>(context), ISupplierRepository
{
    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .Include(s => s.PurchaseOrders)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .Include(s => s.PurchaseOrders)
            .FirstOrDefaultAsync(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .Include(s => s.PurchaseOrders)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .Include(s => s.PurchaseOrders)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Supplier> suppliers, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => 
                s.Name.Contains(search) ||
                s.Code.Contains(search) ||
                (s.ContactPerson != null && s.ContactPerson.Contains(search)) ||
                (s.Email != null && s.Email.Contains(search)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var suppliers = await query
            .OrderBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (suppliers, totalCount);
    }

    public async Task<Supplier> CreateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        supplier.CreatedAt = DateTime.UtcNow;
        supplier.UpdatedAt = DateTime.UtcNow;
        
        await Context.Suppliers.AddAsync(supplier, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        
        return supplier;
    }

    public async Task<Supplier> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        supplier.UpdatedAt = DateTime.UtcNow;
        
        Context.Suppliers.Update(supplier);
        await Context.SaveChangesAsync(cancellationToken);
        
        return supplier;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await GetByIdAsync(id, cancellationToken);
        if (supplier == null)
            return false;

        Context.Suppliers.Remove(supplier);
        await Context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .AnyAsync(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .AnyAsync(s => s.Email != null && s.Email.Equals(email, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers.CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Suppliers
            .CountAsync(s => s.IsActive, cancellationToken);
    }
}
