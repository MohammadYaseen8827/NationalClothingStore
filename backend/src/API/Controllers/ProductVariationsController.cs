using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Controller for product variation operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductVariationsController : ControllerBase
{
    private readonly IProductCatalogService _productCatalogService;
    private readonly ILogger<ProductVariationsController> _logger;

    public ProductVariationsController(
        IProductCatalogService productCatalogService,
        ILogger<ProductVariationsController> logger)
    {
        _productCatalogService = productCatalogService;
        _logger = logger;
    }

    /// <summary>
    /// Get product variation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductVariation>> GetVariation(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var variation = await _productCatalogService.GetProductVariationAsync(id, cancellationToken);
            if (variation == null)
            {
                return NotFound(new ErrorResponse { Message = $"Product variation with ID {id} not found" });
            }
            return Ok(variation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product variation {VariationId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving the product variation" });
        }
    }

    /// <summary>
    /// Get product variation by SKU
    /// </summary>
    [HttpGet("by-sku/{sku}")]
    public async Task<ActionResult<ProductVariation>> GetVariationBySku(
        string sku,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var variation = await _productCatalogService.GetProductVariationBySkuAsync(sku, cancellationToken);
            if (variation == null)
            {
                return NotFound(new ErrorResponse { Message = $"Product variation with SKU '{sku}' not found" });
            }
            return Ok(variation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product variation by SKU {Sku}", sku);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving the product variation" });
        }
    }

    /// <summary>
    /// Update product variation stock
    /// </summary>
    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(
        Guid id,
        [FromBody] UpdateStockRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _productCatalogService.UpdateVariationStockAsync(id, request.Quantity, cancellationToken);
            return Ok(new { Message = "Stock updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for variation {VariationId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while updating stock" });
        }
    }

    /// <summary>
    /// Update product variation
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductVariation>> UpdateVariation(
        Guid id,
        [FromBody] UpdateProductVariationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var variation = await _productCatalogService.UpdateProductVariationAsync(id, request, cancellationToken);
            return Ok(variation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product variation {VariationId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while updating the product variation" });
        }
    }

    /// <summary>
    /// Delete product variation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVariation(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _productCatalogService.DeleteProductVariationAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product variation {VariationId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while deleting the product variation" });
        }
    }
}
