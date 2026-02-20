using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for Customer entity operations
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Get customer by ID with navigation properties
    /// </summary>
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customer by email
    /// </summary>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customer by phone number
    /// </summary>
    Task<Customer?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customers with pagination and filtering
    /// </summary>
    Task<(IEnumerable<Customer> customers, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all customers
    /// </summary>
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get active customers only
    /// </summary>
    Task<IEnumerable<Customer>> GetActiveAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new customer
    /// </summary>
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing customer
    /// </summary>
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a customer (soft delete)
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if customer exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if email already exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if phone number already exists
    /// </summary>
    Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customer count
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get active customer count
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Search customers by name, email, or phone
    /// </summary>
    Task<(IEnumerable<Customer> customers, int totalCount)> SearchAsync(
        string searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get customers with upcoming birthdays
    /// </summary>
    Task<IEnumerable<Customer>> GetCustomersWithUpcomingBirthdaysAsync(
        int daysAhead = 7,
        CancellationToken cancellationToken = default);
}