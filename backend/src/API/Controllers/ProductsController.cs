using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Controller for product management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductCatalogService _productCatalogService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductCatalogService productCatalogService, 
        ILogger<ProductsController> logger)
    {
        _productCatalogService = productCatalogService;
        _logger = logger;
    }

    private List<ValidationError> GetValidationErrors(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
    {
        var errors = new List<ValidationError>();
        foreach (var state in modelState)
        {
            if (state.Value.Errors.Any())
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.Add(new ValidationError
                    {
                        Field = state.Key,
                        Message = error.ErrorMessage
                    });
                }
            }
        }
        return errors;
    }

    /// <summary>
    /// Get all products with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="search">Search term</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="brand">Filter by brand</param>
    /// <param name="season">Filter by season</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    public async Task<ActionResult<(IEnumerable<Product> products, PaginationMetadata pagination)>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? brand = null,
        [FromQuery] string? season = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetProductsRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                CategoryId = categoryId,
                Search = search,
                IsActive = isActive,
                Brand = brand,
                Season = season
            };

            var (products, pagination) = await _productCatalogService.GetProductsAsync(request, cancellationToken);
            return Ok(new { products, pagination });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving products" });
        }
    }

    /// <summary>
    /// Search products
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="includeInactive">Include inactive products</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("search")]
    public async Task<ActionResult<(IEnumerable<Product> products, PaginationMetadata pagination)>> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new SearchProductsRequest
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize,
                IncludeInactive = includeInactive
            };

            var (products, pagination) = await _productCatalogService.SearchProductsAsync(request, cancellationToken);
            return Ok(new { products, pagination });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while searching products" });
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productCatalogService.GetProductAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound(new ErrorResponse { Message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving the product" });
        }
    }

    /// <summary>
    /// Get product by SKU
    /// </summary>
    /// <param name="sku">Product SKU</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("by-sku/{sku}")]
    public async Task<ActionResult<Product>> GetProductBySku(
        string sku,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productCatalogService.GetProductBySkuAsync(sku, cancellationToken);
            if (product == null)
            {
                return NotFound(new ErrorResponse { Message = $"Product with SKU {sku} not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product by SKU {Sku}", sku);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving the product" });
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationErrorResponse { ValidationErrors = GetValidationErrors(ModelState) });
            }

            var product = await _productCatalogService.CreateProductAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating product");
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while creating the product" });
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Product update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationErrorResponse { ValidationErrors = GetValidationErrors(ModelState) });
            }

            var product = await _productCatalogService.UpdateProductAsync(id, request, cancellationToken);
            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating product {ProductId}", id);
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while updating the product" });
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _productCatalogService.DeleteProductAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting product {ProductId}", id);
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while deleting the product" });
        }
    }

    /// <summary>
    /// Get product variations
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="size">Filter by size</param>
    /// <param name="color">Filter by color</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{productId}/variations")]
    public async Task<ActionResult<(IEnumerable<ProductVariation> variations, PaginationMetadata pagination)>> GetProductVariations(
        Guid productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? size = null,
        [FromQuery] string? color = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetProductVariationsRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Size = size,
                Color = color,
                IsActive = isActive
            };

            var (variations, pagination) = await _productCatalogService.GetProductVariationsAsync(productId, request, cancellationToken);
            return Ok(new { variations, pagination });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving variations for product {ProductId}", productId);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving product variations" });
        }
    }

    /// <summary>
    /// Get available sizes for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{productId}/sizes")]
    public async Task<ActionResult<IEnumerable<string>>> GetAvailableSizes(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sizes = await _productCatalogService.GetAvailableSizesAsync(productId, cancellationToken);
            return Ok(sizes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available sizes for product {ProductId}", productId);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving available sizes" });
        }
    }

    /// <summary>
    /// Get available colors for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{productId}/colors")]
    public async Task<ActionResult<IEnumerable<string>>> GetAvailableColors(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var colors = await _productCatalogService.GetAvailableColorsAsync(productId, cancellationToken);
            return Ok(colors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available colors for product {ProductId}", productId);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving available colors" });
        }
    }

    /// <summary>
    /// Get total stock for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{productId}/total-stock")]
    public async Task<ActionResult<int>> GetTotalStock(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var totalStock = await _productCatalogService.GetTotalStockForProductAsync(productId, cancellationToken);
            return Ok(totalStock);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving total stock for product {ProductId}", productId);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving total stock" });
        }
    }

    /// <summary>
    /// Get low stock variations
    /// </summary>
    /// <param name="threshold">Stock threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductVariation>>> GetLowStockVariations(
        [FromQuery] int threshold = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var variations = await _productCatalogService.GetLowStockVariationsAsync(threshold, cancellationToken);
            return Ok(variations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving low stock variations with threshold {Threshold}", threshold);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving low stock variations" });
        }
    }

    /// <summary>
    /// Validate if product can be deleted
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}/validate-deletion")]
    public async Task<ActionResult<bool>> ValidateProductDeletion(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canDelete = await _productCatalogService.ValidateProductDeletionAsync(id, cancellationToken);
            return Ok(canDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating product deletion {ProductId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while validating product deletion" });
        }
    }
}
