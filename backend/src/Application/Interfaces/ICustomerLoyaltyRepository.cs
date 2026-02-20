using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for CustomerLoyalty entity operations
/// </summary>
public interface ICustomerLoyaltyRepository
{
    /// <summary>
    /// Get loyalty account by customer ID
    /// </summary>
    Task<CustomerLoyalty?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get loyalty account by ID
    /// </summary>
    Task<CustomerLoyalty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new loyalty account
    /// </summary>
    Task<CustomerLoyalty> AddAsync(CustomerLoyalty loyalty, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing loyalty account
    /// </summary>
    Task<CustomerLoyalty> UpdateAsync(CustomerLoyalty loyalty, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a loyalty account
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get loyalty accounts with pagination
    /// </summary>
    Task<(IEnumerable<CustomerLoyalty> loyalties, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get loyalty accounts by points balance range
    /// </summary>
    Task<IEnumerable<CustomerLoyalty>> GetByPointsRangeAsync(
        int minPoints,
        int maxPoints,
        CancellationToken cancellationToken = default);
}
