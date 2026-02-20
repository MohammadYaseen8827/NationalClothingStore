using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Controller for category management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IProductCatalogService _productCatalogService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        IProductCatalogService productCatalogService, 
        ILogger<CategoriesController> logger) 
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
    /// Get all categories
    /// </summary>
    /// <param name="includeHierarchy">Include category hierarchy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories(
        [FromQuery] bool includeHierarchy = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _productCatalogService.GetCategoriesAsync(includeHierarchy, cancellationToken);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving categories" });
        }
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _productCatalogService.GetCategoryAsync(id, cancellationToken);
            if (category == null)
            {
                return NotFound(new ErrorResponse { Message = $"Category with ID {id} not found" });
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {CategoryId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving the category" });
        }
    }

    /// <summary>
    /// Get root categories (categories without parent)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("root")]
    public async Task<ActionResult<IEnumerable<Category>>> GetRootCategories(CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _productCatalogService.GetRootCategoriesAsync(cancellationToken);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root categories");
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving root categories" });
        }
    }

    /// <summary>
    /// Get child categories of a parent category
    /// </summary>
    /// <param name="parentId">Parent category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{parentId}/children")]
    public async Task<ActionResult<IEnumerable<Category>>> GetChildCategories(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _productCatalogService.GetChildCategoriesAsync(parentId, cancellationToken);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving child categories for parent {ParentId}", parentId);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while retrieving child categories" });
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="request">Category creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationErrorResponse { Errors = GetValidationErrors(ModelState) });
            }

            var category = await _productCatalogService.CreateCategoryAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating category");
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while creating the category" });
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Category update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{id}")]
    public async Task<ActionResult<Category>> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationErrorResponse { Errors = GetValidationErrors(ModelState) });
            }

            var category = await _productCatalogService.UpdateCategoryAsync(id, request, cancellationToken);
            return Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating category {CategoryId}", id);
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while updating the category" });
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _productCatalogService.DeleteCategoryAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting category {CategoryId}", id);
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while deleting the category" });
        }
    }

    /// <summary>
    /// Validate if category can be deleted
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}/validate-deletion")]
    public async Task<ActionResult<bool>> ValidateCategoryDeletion(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canDelete = await _productCatalogService.ValidateCategoryDeletionAsync(id, cancellationToken);
            return Ok(canDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating category deletion {CategoryId}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while validating category deletion" });
        }
    }
}
