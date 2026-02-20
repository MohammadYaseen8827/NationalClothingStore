using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for Category entity operations
/// </summary>
public class CategoryRepository(NationalClothingStoreDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Include(c => c.ChildCategories)
            .Where(c => c.ParentCategoryId == null && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Include(c => c.ChildCategories)
            .Where(c => c.ParentCategoryId == parentId && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        // Ensure unique code if provided
        if (!string.IsNullOrEmpty(category.Code))
        {
            var existingCategory = await GetByCodeAsync(category.Code, cancellationToken);
            if (existingCategory != null)
            {
                throw new InvalidOperationException($"Category with code '{category.Code}' already exists.");
            }
        }

        // Validate parent category exists if specified
        if (category.ParentCategoryId.HasValue)
        {
            var parentCategory = await GetByIdAsync(category.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
            {
                throw new InvalidOperationException($"Parent category with ID '{category.ParentCategoryId.Value}' not found.");
            }
        }

        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(category.Id, cancellationToken) ?? category;
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var existingCategory = await GetByIdAsync(category.Id, cancellationToken);
        if (existingCategory == null)
        {
            throw new InvalidOperationException($"Category with ID '{category.Id}' not found.");
        }

        // Validate parent category exists if specified and different from current
        if (category.ParentCategoryId.HasValue && category.ParentCategoryId != existingCategory.ParentCategoryId)
        {
            if (category.ParentCategoryId == category.Id)
            {
                throw new InvalidOperationException("Category cannot be its own parent.");
            }

            var parentCategory = await GetByIdAsync(category.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
            {
                throw new InvalidOperationException($"Parent category with ID '{category.ParentCategoryId.Value}' not found.");
            }

            // Check for circular reference
            if (await WouldCreateCircularReference(category.Id, category.ParentCategoryId.Value, cancellationToken))
            {
                throw new InvalidOperationException("Setting this parent would create a circular reference.");
            }
        }

        // Update properties
        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;
        existingCategory.Code = category.Code;
        existingCategory.ParentCategoryId = category.ParentCategoryId;
        existingCategory.SortOrder = category.SortOrder;
        existingCategory.IsActive = category.IsActive;
        existingCategory.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(existingCategory.Id, cancellationToken) ?? existingCategory;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new InvalidOperationException($"Category with ID '{id}' not found.");
        }

        // Check if category has child categories
        if (category.ChildCategories.Any())
        {
            throw new InvalidOperationException("Cannot delete category with child categories. Delete or move child categories first.");
        }

        // Check if category has products
        if (category.Products.Any())
        {
            throw new InvalidOperationException("Cannot delete category with products. Move or delete products first.");
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await context.Categories.AnyAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<bool> HasChildCategoriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AnyAsync(c => c.ParentCategoryId == id && c.IsActive, cancellationToken);
    }

    public async Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .AnyAsync(p => p.CategoryId == id && p.IsActive, cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories.CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories.CountAsync(c => c.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetHierarchyAsync(Guid? rootId = null, CancellationToken cancellationToken = default)
    {
        if (rootId.HasValue)
        {
            // Get hierarchy starting from specific root
            var rootCategory = await GetByIdAsync(rootId.Value, cancellationToken);
            if (rootCategory == null)
            {
                return Enumerable.Empty<Category>();
            }
            
            return await GetCategoryHierarchyRecursive(rootCategory, cancellationToken);
        }

        // Get all root categories and their hierarchies
        var rootCategories = await GetRootCategoriesAsync(cancellationToken);
        var allCategories = new List<Category>();

        foreach (var root in rootCategories)
        {
            var hierarchy = await GetCategoryHierarchyRecursive(root, cancellationToken);
            allCategories.AddRange(hierarchy);
        }

        return allCategories;
    }

    public async Task<IEnumerable<Category>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetActiveAsync(cancellationToken);
        }

        return await context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .Where(c => c.IsActive && (
                c.Name.Contains(searchTerm) ||
                (c.Description != null && c.Description.Contains(searchTerm)) ||
                (c.Code != null && c.Code.Contains(searchTerm))
            ))
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    private async Task<List<Category>> GetCategoryHierarchyRecursive(Category parent, CancellationToken cancellationToken)
    {
        var hierarchy = new List<Category> { parent };
        
        var children = await context.Categories
            .Include(c => c.ChildCategories)
            .Where(c => c.ParentCategoryId == parent.Id && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        foreach (var child in children)
        {
            hierarchy.AddRange(await GetCategoryHierarchyRecursive(child, cancellationToken));
        }

        return hierarchy;
    }

    private async Task<bool> WouldCreateCircularReference(Guid categoryId, Guid newParentId, CancellationToken cancellationToken)
    {
        // Check if the new parent is already a descendant of the category
        var descendants = await GetCategoryHierarchyRecursive(
            await GetByIdAsync(categoryId, cancellationToken) ?? 
            throw new InvalidOperationException($"Category with ID '{categoryId}' not found."),
            cancellationToken
        );

        return descendants.Any(c => c.Id == newParentId);
    }
}
