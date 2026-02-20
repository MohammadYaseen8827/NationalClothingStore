using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for ProductVariation entity operations
/// </summary>
public interface IProductVariationRepository
{
    /// <summary>
    /// Get product variation by ID with navigation properties
    /// </summary>
    Task<ProductVariation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product variation by SKU
    /// </summary>
    Task<ProductVariation?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get variations for a specific product
    /// </summary>
    Task<IEnumerable<ProductVariation>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get variations for a specific product with pagination
    /// </summary>
    Task<(IEnumerable<ProductVariation> variations, int totalCount)> GetByProductIdPagedAsync(
        Guid productId,
        int pageNumber,
        int pageSize,
        string? size = null,
        string? color = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all product variations
    /// </summary>
    Task<IEnumerable<ProductVariation>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active product variations only
    /// </summary>
    Task<IEnumerable<ProductVariation>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new product variation
    /// </summary>
    Task<ProductVariation> CreateAsync(ProductVariation variation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing product variation
    /// </summary>
    Task<ProductVariation> UpdateAsync(ProductVariation variation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a product variation
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if variation exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if variation SKU exists
    /// </summary>
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if variation with same size and color exists for product
    /// </summary>
    Task<bool> SizeColorCombinationExistsAsync(Guid productId, string size, string color, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total count of variations
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of active variations
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get variations by size
    /// </summary>
    Task<IEnumerable<ProductVariation>> GetBySizeAsync(string size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get variations by color
    /// </summary>
    Task<IEnumerable<ProductVariation>> GetByColorAsync(string color, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get variations with low stock
    /// </summary>
    Task<IEnumerable<ProductVariation>> GetLowStockVariationsAsync(int threshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get out of stock variations
    /// </summary>
    Task<IEnumerable<ProductVariation>> GetOutOfStockVariationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Update stock quantity for a variation
    /// </summary>
    Task UpdateStockAsync(Guid id, int newQuantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available sizes for a product
    /// </summary>
    Task<IEnumerable<string>> GetAvailableSizesAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available colors for a product
    /// </summary>
    Task<IEnumerable<string>> GetAvailableColorsAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total stock for a product across all variations
    /// </summary>
    Task<int> GetTotalStockForProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update variation status (active/inactive)
    /// </summary>
    Task UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
}
