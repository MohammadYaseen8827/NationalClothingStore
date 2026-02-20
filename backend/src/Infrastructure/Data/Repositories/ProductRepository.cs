using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for Product entity operations
/// </summary>
public class ProductRepository(NationalClothingStoreDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .Include(p => p.Inventories)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task<(IEnumerable<Product> products, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? categoryId = null,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .AsQueryable();

        // Apply filters
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => 
                p.Name.Contains(search) ||
                (p.Description != null && p.Description.Contains(search)) ||
                p.SKU.Contains(search) ||
                (p.Brand != null && p.Brand.Contains(search))
            );
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        // Ensure unique SKU
        var existingProduct = await GetBySkuAsync(product.SKU, cancellationToken);
        if (existingProduct != null)
        {
            throw new InvalidOperationException($"Product with SKU '{product.SKU}' already exists.");
        }

        // Validate category exists
        var category = await context.Categories.FindAsync(new object[] { product.CategoryId }, cancellationToken);
        if (category == null)
        {
            throw new InvalidOperationException($"Category with ID '{product.CategoryId}' not found.");
        }

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(product.Id, cancellationToken) ?? product;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existingProduct = await GetByIdAsync(product.Id, cancellationToken);
        if (existingProduct == null)
        {
            throw new InvalidOperationException($"Product with ID '{product.Id}' not found.");
        }

        // Check SKU uniqueness if changed
        if (product.SKU != existingProduct.SKU)
        {
            var skuExists = await SkuExistsAsync(product.SKU, cancellationToken);
            if (skuExists)
            {
                throw new InvalidOperationException($"Product with SKU '{product.SKU}' already exists.");
            }
        }

        // Validate category exists if changed
        if (product.CategoryId != existingProduct.CategoryId)
        {
            var category = await context.Categories.FindAsync(new object[] { product.CategoryId }, cancellationToken);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID '{product.CategoryId}' not found.");
            }
        }

        // Update properties
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.SKU = product.SKU;
        existingProduct.Barcode = product.Barcode;
        existingProduct.BasePrice = product.BasePrice;
        existingProduct.CostPrice = product.CostPrice;
        existingProduct.Brand = product.Brand;
        existingProduct.Season = product.Season;
        existingProduct.Material = product.Material;
        existingProduct.Color = product.Color;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.IsActive = product.IsActive;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(existingProduct.Id, cancellationToken) ?? existingProduct;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID '{id}' not found.");
        }

        // Soft delete by setting IsActive to false
        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await context.Products.AnyAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task<bool> HasVariationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ProductVariations
            .AnyAsync(pv => pv.ProductId == id && pv.IsActive, cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products.CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products.CountAsync(p => p.IsActive, cancellationToken);
    }

    public async Task<(IEnumerable<Product> products, int totalCount)> SearchAsync(
        string searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetPagedAsync(pageNumber, pageSize, isActive: true, cancellationToken: cancellationToken);
        }

        var query = context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .Where(p => p.IsActive && (
                p.Name.Contains(searchTerm) ||
                (p.Description != null && p.Description.Contains(searchTerm)) ||
                p.SKU.Contains(searchTerm) ||
                (p.Brand != null && p.Brand.Contains(searchTerm)) ||
                (p.Season != null && p.Season.Contains(searchTerm)) ||
                (p.Material != null && p.Material.Contains(searchTerm))
            ));

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<IEnumerable<Product>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .Where(p => p.Brand == brand && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetBySeasonAsync(string season, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .Where(p => p.Season == season && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default)
    {
        // Get products where total stock across all variations is below threshold
        var lowStockVariations = await context.ProductVariations
            .Where(pv => pv.IsActive && pv.StockQuantity <= threshold)
            .ToListAsync(cancellationToken);

        var productIds = lowStockVariations.Select(pv => pv.ProductId).Distinct();

        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Variations)
            .Include(p => p.Images)
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var product = await context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID '{id}' not found.");
        }

        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}
