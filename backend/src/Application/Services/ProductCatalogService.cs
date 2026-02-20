using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;
using NationalClothingStore.Application.Common;
using Microsoft.Extensions.Logging;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service for product catalog management operations
/// </summary>
public class ProductCatalogService(
    ICategoryRepository categoryRepository,
    IProductRepository productRepository,
    IProductVariationRepository productVariationRepository,
    ILogger<ProductCatalogService> logger,
    IBranchRepository branchRepository,
    IWarehouseRepository warehouseRepository) : IProductCatalogService
{
    // Category Management
    public async Task<Category> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating category: {Name}", request.Name);

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            ParentCategoryId = request.ParentId,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        return await categoryRepository.CreateAsync(category, cancellationToken);
    }

    public async Task<Category> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating category: {Id}", id);

        var category = new Category
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            ParentCategoryId = request.ParentId,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        return await categoryRepository.UpdateAsync(category, cancellationToken);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting category: {Id}", id);

        var canDelete = await ValidateCategoryDeletionAsync(id, cancellationToken);
        if (!canDelete)
        {
            throw new InvalidOperationException("Cannot delete category with child categories or products.");
        }

        await categoryRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<Category?> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await categoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(bool includeHierarchy = false, CancellationToken cancellationToken = default)
    {
        if (includeHierarchy)
        {
            return await categoryRepository.GetHierarchyAsync(cancellationToken: cancellationToken);
        }

        return await categoryRepository.GetActiveAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await categoryRepository.GetRootCategoriesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await categoryRepository.GetChildCategoriesAsync(parentId, cancellationToken);
    }

    // Product Management
    public async Task<Product> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating product: {Name}", request.Name);

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            SKU = request.SKU,
            Barcode = request.Barcode,
            BasePrice = request.BasePrice,
            CostPrice = request.CostPrice,
            Brand = request.Brand,
            Season = request.Season,
            Material = request.Material,
            Color = request.Color,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive
        };

        return await productRepository.CreateAsync(product, cancellationToken);
    }

    public async Task<Product> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating product: {Id}", id);

        var product = new Product
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Barcode = request.Barcode,
            BasePrice = request.BasePrice,
            CostPrice = request.CostPrice,
            Brand = request.Brand,
            Season = request.Season,
            Material = request.Material,
            Color = request.Color,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive
        };

        return await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting product: {Id}", id);

        var canDelete = await ValidateProductDeletionAsync(id, cancellationToken);
        if (!canDelete)
        {
            throw new InvalidOperationException("Cannot delete product with active variations or inventory.");
        }

        await productRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<Product?> GetProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetBySkuAsync(sku, cancellationToken);
    }

    public async Task<(IEnumerable<Product> products, PaginationMetadata pagination)> GetProductsAsync(
        GetProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        var (products, totalCount) = await productRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.CategoryId,
            request.Search,
            request.IsActive,
            cancellationToken);

        var pagination = new PaginationMetadata
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize)
        };

        return (products, pagination);
    }

    public async Task<(IEnumerable<Product> products, PaginationMetadata pagination)> SearchProductsAsync(
        SearchProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        var (products, totalCount) = await productRepository.SearchAsync(
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var pagination = new PaginationMetadata
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize)
        };

        return (products, pagination);
    }

    // Product Variation Management
    public async Task<ProductVariation> CreateProductVariationAsync(CreateProductVariationRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating product variation: {SKU}", request.SKU);

        var validationResult = await ValidateProductVariationAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(string.Join(", ", validationResult.Errors));
        }

        var variation = new ProductVariation
        {
            ProductId = request.ProductId,
            Size = request.Size,
            Color = request.Color,
            SKU = request.SKU,
            AdditionalPrice = request.AdditionalPrice,
            CostPrice = request.CostPrice,
            StockQuantity = request.StockQuantity,
            IsActive = request.IsActive
        };

        return await productVariationRepository.CreateAsync(variation, cancellationToken);
    }

    public async Task<ProductVariation> UpdateProductVariationAsync(Guid id, UpdateProductVariationRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating product variation: {Id}", id);

        var variation = new ProductVariation
        {
            Id = id,
            Size = request.Size,
            Color = request.Color,
            SKU = request.SKU,
            AdditionalPrice = request.AdditionalPrice,
            CostPrice = request.CostPrice,
            StockQuantity = request.StockQuantity,
            IsActive = request.IsActive
        };

        return await productVariationRepository.UpdateAsync(variation, cancellationToken);
    }

    public async Task DeleteProductVariationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting product variation: {Id}", id);
        await productVariationRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ProductVariation?> GetProductVariationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await productVariationRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<ProductVariation?> GetProductVariationBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await productVariationRepository.GetBySkuAsync(sku, cancellationToken);
    }

    public async Task<(IEnumerable<ProductVariation> variations, PaginationMetadata pagination)> GetProductVariationsAsync(
        Guid productId,
        GetProductVariationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var (variations, totalCount) = await productVariationRepository.GetByProductIdPagedAsync(
            productId,
            request.PageNumber,
            request.PageSize,
            request.Size,
            request.Color,
            request.IsActive,
            cancellationToken);

        var pagination = new PaginationMetadata
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize)
        };

        return (variations, pagination);
    }

    public async Task<IEnumerable<string>> GetAvailableSizesAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await productVariationRepository.GetAvailableSizesAsync(productId, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAvailableColorsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await productVariationRepository.GetAvailableColorsAsync(productId, cancellationToken);
    }

    // Product Image Management
    public async Task<ProductImage> AddProductImageAsync(AddProductImageRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding product image to product: {ProductId}", request.ProductId);

        var image = new ProductImage
        {
            ProductId = request.ProductId,
            ImageUrl = request.ImageUrl,
            AltText = request.AltText,
            Caption = request.Caption,
            SortOrder = request.SortOrder,
            IsPrimary = request.IsPrimary
        };

        // If this is set as primary, unset other primary images
        if (request.IsPrimary)
        {
            var existingImages = await GetProductImagesAsync(request.ProductId, cancellationToken);
            foreach (var existingImage in existingImages.Where(i => i.IsPrimary))
            {
                existingImage.IsPrimary = false;
                // Note: In a real implementation, you'd update these via repository
            }
        }

        return await productRepository.CreateAsync(new Product
        {
            Id = request.ProductId,
            Images = new List<ProductImage> { image }
        }, cancellationToken).ContinueWith(t => image, cancellationToken);
    }

    public async Task<ProductImage> UpdateProductImageAsync(Guid id, UpdateProductImageRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating product image: {Id}", id);

        // Note: This would need a dedicated ProductImageRepository in a real implementation
        throw new NotImplementedException("Product image repository not yet implemented");
    }

    public async Task DeleteProductImageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting product image: {Id}", id);
        // Note: This would need a dedicated ProductImageRepository in a real implementation
        throw new NotImplementedException("Product image repository not yet implemented");
    }

    public async Task<ProductImage?> GetProductImageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Note: This would need a dedicated ProductImageRepository in a real implementation
        throw new NotImplementedException("Product image repository not yet implemented");
    }

    public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        return product?.Images ?? Enumerable.Empty<ProductImage>();
    }

    public async Task<ProductImage?> GetPrimaryImageAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var images = await GetProductImagesAsync(productId, cancellationToken);
        return images.FirstOrDefault(i => i.IsPrimary);
    }

    // Inventory Management
    public async Task UpdateVariationStockAsync(Guid variationId, int quantity, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating stock for variation {Id} to {Quantity}", variationId, quantity);
        await productVariationRepository.UpdateStockAsync(variationId, quantity, cancellationToken);
    }

    public async Task<int> GetTotalStockForProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await productVariationRepository.GetTotalStockForProductAsync(productId, cancellationToken);
    }

    public async Task<IEnumerable<ProductVariation>> GetLowStockVariationsAsync(int threshold, CancellationToken cancellationToken = default)
    {
        return await productVariationRepository.GetLowStockVariationsAsync(threshold, cancellationToken);
    }

    // Validation and Business Logic
    public async Task<bool> ValidateCategoryDeletionAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var hasChildren = await categoryRepository.HasChildCategoriesAsync(categoryId, cancellationToken);
        var hasProducts = await categoryRepository.HasProductsAsync(categoryId, cancellationToken);

        return !hasChildren && !hasProducts;
    }

    public async Task<bool> ValidateProductDeletionAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var hasVariations = await productRepository.HasVariationsAsync(productId, cancellationToken);
        return !hasVariations;
    }

    public async Task<ValidationResult> ValidateProductVariationAsync(CreateProductVariationRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Validate product exists
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            errors.Add("Product not found.");
        }

        // Validate SKU uniqueness
        if (await productVariationRepository.SkuExistsAsync(request.SKU, cancellationToken))
        {
            errors.Add("SKU already exists.");
        }

        // Validate size/color combination
        if (await productVariationRepository.SizeColorCombinationExistsAsync(
            request.ProductId, 
            request.Size, 
            request.Color, 
            cancellationToken))
        {
            errors.Add("A variation with this size and color already exists for this product.");
        }

        // Validate stock quantity
        if (request.StockQuantity < 0)
        {
            errors.Add("Stock quantity cannot be negative.");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    // Branch and Warehouse Management
    public async Task<Branch?> GetBranchAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting branch with ID: {BranchId}", id);
        return await branchRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Warehouse?> GetWarehouseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting warehouse with ID: {WarehouseId}", id);
        return await warehouseRepository.GetByIdAsync(id, cancellationToken);
    }
}
