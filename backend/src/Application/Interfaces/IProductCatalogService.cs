using NationalClothingStore.Domain.Entities;
using NationalClothingStore.Application.Common;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Service interface for product catalog management operations
/// </summary>
public interface IProductCatalogService
{
    // Category Management
    Task<Category> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Category> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoriesAsync(bool includeHierarchy = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);

    // Product Management
    Task<Product> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Product> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> products, PaginationMetadata pagination)> GetProductsAsync(
        GetProductsRequest request,
        CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> products, PaginationMetadata pagination)> SearchProductsAsync(
        SearchProductsRequest request,
        CancellationToken cancellationToken = default);

    // Product Variation Management
    Task<ProductVariation> CreateProductVariationAsync(CreateProductVariationRequest request, CancellationToken cancellationToken = default);
    Task<ProductVariation> UpdateProductVariationAsync(Guid id, UpdateProductVariationRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductVariationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductVariation?> GetProductVariationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductVariation?> GetProductVariationBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<(IEnumerable<ProductVariation> variations, PaginationMetadata pagination)> GetProductVariationsAsync(
        Guid productId,
        GetProductVariationsRequest request,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAvailableSizesAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAvailableColorsAsync(Guid productId, CancellationToken cancellationToken = default);

    // Product Image Management
    Task<ProductImage> AddProductImageAsync(AddProductImageRequest request, CancellationToken cancellationToken = default);
    Task<ProductImage> UpdateProductImageAsync(Guid id, UpdateProductImageRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductImageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductImage?> GetProductImageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductImage>> GetProductImagesAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductImage?> GetPrimaryImageAsync(Guid productId, CancellationToken cancellationToken = default);

    // Inventory Management
    Task UpdateVariationStockAsync(Guid variationId, int quantity, CancellationToken cancellationToken = default);
    Task<int> GetTotalStockForProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductVariation>> GetLowStockVariationsAsync(int threshold, CancellationToken cancellationToken = default);

    // Validation and Business Logic
    Task<bool> ValidateCategoryDeletionAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<bool> ValidateProductDeletionAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateProductVariationAsync(CreateProductVariationRequest request, CancellationToken cancellationToken = default);

    // Branch and Warehouse Management
    Task<Branch?> GetBranchAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetWarehouseAsync(Guid id, CancellationToken cancellationToken = default);
}

// Request/Response DTOs
public record CreateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Code { get; init; }
    public Guid? ParentId { get; init; }
    public int SortOrder { get; init; } = 0;
    public bool IsActive { get; init; } = true;
};

public record UpdateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Code { get; init; }
    public Guid? ParentId { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
};

public record CreateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal BasePrice { get; init; }
    public decimal CostPrice { get; init; }
    public string? Brand { get; init; }
    public string? Season { get; init; }
    public string? Material { get; init; }
    public string? Color { get; init; }
    public Guid CategoryId { get; init; }
    public bool IsActive { get; init; } = true;
};

public record UpdateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Barcode { get; init; }
    public decimal BasePrice { get; init; }
    public decimal CostPrice { get; init; }
    public string? Brand { get; init; }
    public string? Season { get; init; }
    public string? Material { get; init; }
    public string? Color { get; init; }
    public Guid CategoryId { get; init; }
    public bool IsActive { get; init; }
};

public record CreateProductVariationRequest
{
    public Guid ProductId { get; init; }
    public string Size { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public decimal AdditionalPrice { get; init; } = 0m;
    public decimal CostPrice { get; init; }
    public int StockQuantity { get; init; } = 0;
    public bool IsActive { get; init; } = true;
};

public record UpdateProductVariationRequest
{
    public string Size { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public decimal AdditionalPrice { get; init; } = 0m;
    public decimal CostPrice { get; init; }
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; } = true;
};

public record GetProductsRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public Guid? CategoryId { get; init; }
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public string? Brand { get; init; }
    public string? Season { get; init; }
};

public record SearchProductsRequest
{
    public string SearchTerm { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool IncludeInactive { get; init; } = false;
};

public record GetProductVariationsRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Size { get; init; }
    public string? Color { get; init; }
    public bool? IsActive { get; init; }
};

public record AddProductImageRequest
{
    public Guid ProductId { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? AltText { get; init; }
    public string? Caption { get; init; }
    public int SortOrder { get; init; } = 0;
    public bool IsPrimary { get; init; } = false;
};

public record UpdateProductImageRequest
{
    public string ImageUrl { get; init; } = string.Empty;
    public string? AltText { get; init; }
    public string? Caption { get; init; }
    public int SortOrder { get; init; }
    public bool IsPrimary { get; init; }
};

public record PaginationMetadata
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
};
