using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for Category entity operations
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Get category by ID with navigation properties
    /// </summary>
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get category by code
    /// </summary>
    Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all categories
    /// </summary>
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active categories only
    /// </summary>
    Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get root categories (categories without parent)
    /// </summary>
    Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get child categories of a specific parent
    /// </summary>
    Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new category
    /// </summary>
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing category
    /// </summary>
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a category
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category code exists
    /// </summary>
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category has child categories
    /// </summary>
    Task<bool> HasChildCategoriesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category has products
    /// </summary>
    Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total count of categories
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of active categories
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get category hierarchy (recursive)
    /// </summary>
    Task<IEnumerable<Category>> GetHierarchyAsync(Guid? rootId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search categories by name, description, or code
    /// </summary>
    Task<IEnumerable<Category>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
