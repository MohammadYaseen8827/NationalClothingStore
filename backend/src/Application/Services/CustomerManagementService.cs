using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service for managing customer profiles and loyalty programs
/// </summary>
public class CustomerManagementService : ICustomerManagementService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerLoyaltyRepository _loyaltyRepository;
    private readonly ILogger<CustomerManagementService> _logger;

    public CustomerManagementService(
        ICustomerRepository customerRepository,
        ICustomerLoyaltyRepository loyaltyRepository,
        ILogger<CustomerManagementService> logger)
    {
        _customerRepository = customerRepository;
        _loyaltyRepository = loyaltyRepository;
        _logger = logger;
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _customerRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}", id);
            throw;
        }
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _customerRepository.GetByEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with email {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var (customers, _) = await _customerRepository.GetPagedAsync(page, pageSize, cancellationToken: cancellationToken);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers list");
            throw;
        }
    }

    public async Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if customer with email already exists
            var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException($"Customer with email {request.Email} already exists");
            }

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                DateOfBirth = request.DateOfBirth,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdCustomer = await _customerRepository.CreateAsync(customer, cancellationToken);
            _logger.LogInformation("Created new customer with ID {CustomerId}", createdCustomer.Id);

            return createdCustomer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer with email {Email}", request.Email);
            throw;
        }
    }

    public async Task<Customer> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found");
            }

            customer.FirstName = request.FirstName ?? customer.FirstName;
            customer.LastName = request.LastName ?? customer.LastName;
            customer.PhoneNumber = request.Phone ?? customer.PhoneNumber;
            customer.DateOfBirth = request.DateOfBirth ?? customer.DateOfBirth;
            customer.UpdatedAt = DateTime.UtcNow;

            var updatedCustomer = await _customerRepository.UpdateAsync(customer, cancellationToken);
            _logger.LogInformation("Updated customer with ID {CustomerId}", id);

            return updatedCustomer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID {CustomerId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found");
            }

            await _customerRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Deleted customer with ID {CustomerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID {CustomerId}", id);
            throw;
        }
    }

    public async Task<CustomerLoyalty?> GetCustomerLoyaltyAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _loyaltyRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving loyalty information for customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<CustomerLoyalty> AddLoyaltyPointsAsync(Guid customerId, int points, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var loyalty = await _loyaltyRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            if (loyalty == null)
            {
                loyalty = new CustomerLoyalty
                {
                    CustomerId = customerId,
                    PointsBalance = points,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                loyalty = await _loyaltyRepository.AddAsync(loyalty, cancellationToken);
            }
            else
            {
                loyalty.PointsBalance += points;
                loyalty.UpdatedAt = DateTime.UtcNow;
                loyalty = await _loyaltyRepository.UpdateAsync(loyalty, cancellationToken);
            }

            _logger.LogInformation("Added {Points} loyalty points to customer {CustomerId} for reason: {Reason}", points, customerId, reason);
            return loyalty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding loyalty points to customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<CustomerLoyalty> RedeemLoyaltyPointsAsync(Guid customerId, int points, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var loyalty = await _loyaltyRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            if (loyalty == null)
            {
                throw new KeyNotFoundException($"Loyalty account not found for customer {customerId}");
            }

            if (loyalty.PointsBalance < points)
            {
                throw new InvalidOperationException($"Insufficient loyalty points. Available: {loyalty.PointsBalance}, Requested: {points}");
            }

            loyalty.PointsBalance -= points;
            loyalty.UpdatedAt = DateTime.UtcNow;
            loyalty = await _loyaltyRepository.UpdateAsync(loyalty, cancellationToken);

            _logger.LogInformation("Redeemed {Points} loyalty points from customer {CustomerId} for reason: {Reason}", points, customerId, reason);
            return loyalty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redeeming loyalty points from customer {CustomerId}", customerId);
            throw;
        }
    }
}

/// <summary>
/// Request model for creating a customer
/// </summary>
public record CreateCustomerRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public DateTime? DateOfBirth { get; init; }
}

/// <summary>
/// Request model for updating a customer
/// </summary>
public record UpdateCustomerRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Phone { get; init; }
    public DateTime? DateOfBirth { get; init; }
}
