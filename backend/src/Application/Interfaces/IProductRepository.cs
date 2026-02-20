using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for Product entity operations
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Get product by ID with navigation properties
    /// </summary>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product by SKU
    /// </summary>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products with pagination and filtering
    /// </summary>
    Task<(IEnumerable<Product> products, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? categoryId = null,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all products
    /// </summary>
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active products only
    /// </summary>
    Task<IEnumerable<Product>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products by category
    /// </summary>
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new product
    /// </summary>
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing product
    /// </summary>
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a product (soft delete)
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if product exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if product SKU exists
    /// </summary>
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if product has variations
    /// </summary>
    Task<bool> HasVariationsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total count of products
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of active products
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Search products by name, description, SKU, brand
    /// </summary>
    Task<(IEnumerable<Product> products, int totalCount)> SearchAsync(
        string searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products by brand
    /// </summary>
    Task<IEnumerable<Product>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products by season
    /// </summary>
    Task<IEnumerable<Product>> GetBySeasonAsync(string season, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products with low stock
    /// </summary>
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update product status (active/inactive)
    /// </summary>
    Task UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
}
