using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for ProductVariation entity operations
/// </summary>
public class ProductVariationRepository : IProductVariationRepository
{
    private readonly NationalClothingStoreDbContext _context;

    public ProductVariationRepository(NationalClothingStoreDbContext context)
    {
        _context = context;
    }

    public async Task<ProductVariation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .FirstOrDefaultAsync(pv => pv.Id == id, cancellationToken);
    }

    public async Task<ProductVariation?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .FirstOrDefaultAsync(pv => pv.SKU == sku, cancellationToken);
    }

    public async Task<IEnumerable<ProductVariation>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .Where(pv => pv.ProductId == productId && pv.IsActive)
            .OrderBy(pv => pv.Size)
            .ThenBy(pv => pv.Color)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<ProductVariation> variations, int totalCount)> GetByProductIdPagedAsync(
        Guid productId,
        int pageNumber,
        int pageSize,
        string? size = null,
        string? color = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .Where(pv => pv.ProductId == productId)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(size))
        {
            query = query.Where(pv => pv.Size == size);
        }

        if (!string.IsNullOrWhiteSpace(color))
        {
            query = query.Where(pv => pv.Color == color);
        }

        if (isActive.HasValue)
        {
            query = query.Where(pv => pv.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var variations = await query
            .OrderBy(pv => pv.Size)
            .ThenBy(pv => pv.Color)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (variations, totalCount);
    }

    public async Task<IEnumerable<ProductVariation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .OrderBy(pv => pv.Product.Name)
            .ThenBy(pv => pv.Size)
            .ThenBy(pv => pv.Color)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductVariation>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .Where(pv => pv.IsActive)
            .OrderBy(pv => pv.Product.Name)
            .ThenBy(pv => pv.Size)
            .ThenBy(pv => pv.Color)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductVariation> CreateAsync(ProductVariation variation, CancellationToken cancellationToken = default)
    {
        // Ensure unique SKU
        var existingVariation = await GetBySkuAsync(variation.SKU, cancellationToken);
        if (existingVariation != null)
        {
            throw new InvalidOperationException($"Product variation with SKU '{variation.SKU}' already exists.");
        }

        // Check for duplicate size/color combination for the same product
        var duplicateExists = await SizeColorCombinationExistsAsync(
            variation.ProductId, 
            variation.Size, 
            variation.Color, 
            cancellationToken);
        
        if (duplicateExists)
        {
            throw new InvalidOperationException($"A variation with size '{variation.Size}' and color '{variation.Color}' already exists for this product.");
        }

        // Validate product exists
        var product = await _context.Products.FindAsync(new object[] { variation.ProductId }, cancellationToken);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID '{variation.ProductId}' not found.");
        }

        _context.ProductVariations.Add(variation);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(variation.Id, cancellationToken) ?? variation;
    }

    public async Task<ProductVariation> UpdateAsync(ProductVariation variation, CancellationToken cancellationToken = default)
    {
        var existingVariation = await GetByIdAsync(variation.Id, cancellationToken);
        if (existingVariation == null)
        {
            throw new InvalidOperationException($"Product variation with ID '{variation.Id}' not found.");
        }

        // Check SKU uniqueness if changed
        if (variation.SKU != existingVariation.SKU)
        {
            var skuExists = await SkuExistsAsync(variation.SKU, cancellationToken);
            if (skuExists)
            {
                throw new InvalidOperationException($"Product variation with SKU '{variation.SKU}' already exists.");
            }
        }

        // Check for duplicate size/color combination if changed (excluding current variation)
        if (variation.Size != existingVariation.Size || variation.Color != existingVariation.Color)
        {
            var duplicateExists = await _context.ProductVariations
                .AnyAsync(pv => 
                    pv.ProductId == variation.ProductId && 
                    pv.Size == variation.Size && 
                    pv.Color == variation.Color && 
                    pv.Id != variation.Id &&
                    pv.IsActive, 
                    cancellationToken);
            
            if (duplicateExists)
            {
                throw new InvalidOperationException($"A variation with size '{variation.Size}' and color '{variation.Color}' already exists for this product.");
            }
        }

        // Update properties
        existingVariation.Size = variation.Size;
        existingVariation.Color = variation.Color;
        existingVariation.SKU = variation.SKU;
        existingVariation.AdditionalPrice = variation.AdditionalPrice;
        existingVariation.CostPrice = variation.CostPrice;
        existingVariation.StockQuantity = variation.StockQuantity;
        existingVariation.IsActive = variation.IsActive;
        existingVariation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(existingVariation.Id, cancellationToken) ?? existingVariation;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var variation = await GetByIdAsync(id, cancellationToken);
        if (variation == null)
        {
            throw new InvalidOperationException($"Product variation with ID '{id}' not found.");
        }

        // Soft delete by setting IsActive to false
        variation.IsActive = false;
        variation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations.AnyAsync(pv => pv.Id == id, cancellationToken);
    }

    public async Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations.AnyAsync(pv => pv.SKU == sku, cancellationToken);
    }

    public async Task<bool> SizeColorCombinationExistsAsync(Guid productId, string size, string color, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .AnyAsync(pv => 
                pv.ProductId == productId && 
                pv.Size == size && 
                pv.Color == color && 
                pv.IsActive, 
                cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations.CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations.CountAsync(pv => pv.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<ProductVariation>> GetBySizeAsync(string size, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .Where(pv => pv.Size == size && pv.IsActive)
            .OrderBy(pv => pv.Product.Name)
            .ThenBy(pv => pv.Color)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductVariation>> GetByColorAsync(string color, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .Where(pv => pv.Color == color && pv.IsActive)
            .OrderBy(pv => pv.Product.Name)
            .ThenBy(pv => pv.Size)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductVariation>> GetLowStockVariationsAsync(int threshold, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .Where(pv => pv.IsActive && pv.StockQuantity <= threshold)
            .OrderBy(pv => pv.StockQuantity)
            .ThenBy(pv => pv.Product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductVariation>> GetOutOfStockVariationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Include(pv => pv.Product)
            .Include(pv => pv.Inventories)
            .Where(pv => pv.IsActive && pv.StockQuantity == 0)
            .OrderBy(pv => pv.Product.Name)
            .ThenBy(pv => pv.Size)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateStockAsync(Guid id, int newQuantity, CancellationToken cancellationToken = default)
    {
        var variation = await _context.ProductVariations.FindAsync(new object[] { id }, cancellationToken);
        if (variation == null)
        {
            throw new InvalidOperationException($"Product variation with ID '{id}' not found.");
        }

        variation.StockQuantity = newQuantity;
        variation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAvailableSizesAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Where(pv => pv.ProductId == productId && pv.IsActive)
            .Select(pv => pv.Size)
            .Distinct()
            .OrderBy(size => size)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAvailableColorsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Where(pv => pv.ProductId == productId && pv.IsActive)
            .Select(pv => pv.Color)
            .Distinct()
            .OrderBy(color => color)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalStockForProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariations
            .Where(pv => pv.ProductId == productId && pv.IsActive)
            .SumAsync(pv => pv.StockQuantity, cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var variation = await _context.ProductVariations.FindAsync(new object[] { id }, cancellationToken);
        if (variation == null)
        {
            throw new InvalidOperationException($"Product variation with ID '{id}' not found.");
        }

        variation.IsActive = isActive;
        variation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
