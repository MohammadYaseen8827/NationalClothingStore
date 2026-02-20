using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for Supplier entity operations
/// </summary>
public interface ISupplierRepository
{
    /// <summary>
    /// Get supplier by ID with navigation properties
    /// </summary>
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get supplier by code
    /// </summary>
    Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all suppliers
    /// </summary>
    Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get active suppliers only
    /// </summary>
    Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get suppliers with pagination and filtering
    /// </summary>
    Task<(IEnumerable<Supplier> suppliers, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new supplier
    /// </summary>
    Task<Supplier> CreateAsync(Supplier supplier, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing supplier
    /// </summary>
    Task<Supplier> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a supplier
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if supplier exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if supplier code already exists
    /// </summary>
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if supplier email already exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get supplier count
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get active supplier count
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
}
