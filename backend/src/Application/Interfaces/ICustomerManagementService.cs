using NationalClothingStore.Application.Services;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Service interface for customer management operations
/// </summary>
public interface ICustomerManagementService
{
    /// <summary>
    /// Get customer by ID
    /// </summary>
    Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customer by email
    /// </summary>
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customers with pagination
    /// </summary>
    Task<IEnumerable<Customer>> GetCustomersAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new customer
    /// </summary>
    Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing customer
    /// </summary>
    Task<Customer> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a customer
    /// </summary>
    Task<bool> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customer loyalty information
    /// </summary>
    Task<CustomerLoyalty?> GetCustomerLoyaltyAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add loyalty points to customer account
    /// </summary>
    Task<CustomerLoyalty> AddLoyaltyPointsAsync(Guid customerId, int points, string reason, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Redeem loyalty points from customer account
    /// </summary>
    Task<CustomerLoyalty> RedeemLoyaltyPointsAsync(Guid customerId, int points, string reason, CancellationToken cancellationToken = default);
}
