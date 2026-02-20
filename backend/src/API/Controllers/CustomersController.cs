using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Controller for customer management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerManagementService _customerManagementService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerManagementService customerManagementService,
        ILogger<CustomersController> logger)
    {
        _customerManagementService = customerManagementService;
        _logger = logger;
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _customerManagementService.GetCustomerByIdAsync(id, cancellationToken);
            if (customer == null)
            {
                return NotFound($"Customer with ID {id} not found");
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customers with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var customers = await _customerManagementService.GetCustomersAsync(page, pageSize, cancellationToken);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers list");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Search customers by email or name
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<Customer>> SearchCustomer([FromQuery] string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _customerManagementService.GetCustomerByEmailAsync(query, cancellationToken);
            if (customer == null)
            {
                return NotFound($"Customer with email {query} not found");
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customer with query {Query}", query);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerManagementService.CreateCustomerAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Customer creation failed: {Message}", ex.Message);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Customer>> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerManagementService.UpdateCustomerAsync(id, request, cancellationToken);
            return Ok(customer);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Customer update failed: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _customerManagementService.DeleteCustomerAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound($"Customer with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customer loyalty information
    /// </summary>
    [HttpGet("{id}/loyalty")]
    public async Task<ActionResult<CustomerLoyalty>> GetCustomerLoyalty(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var loyalty = await _customerManagementService.GetCustomerLoyaltyAsync(id, cancellationToken);
            if (loyalty == null)
            {
                return NotFound($"Loyalty account not found for customer {id}");
            }

            return Ok(loyalty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving loyalty information for customer {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Add loyalty points to customer
    /// </summary>
    [HttpPost("{id}/loyalty/add-points")]
    public async Task<ActionResult<CustomerLoyalty>> AddLoyaltyPoints(
        Guid id, 
        [FromBody] AddLoyaltyPointsRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loyalty = await _customerManagementService.AddLoyaltyPointsAsync(id, request.Points, request.Reason, cancellationToken);
            return Ok(loyalty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding loyalty points for customer {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Redeem loyalty points from customer
    /// </summary>
    [HttpPost("{id}/loyalty/redeem-points")]
    public async Task<ActionResult<CustomerLoyalty>> RedeemLoyaltyPoints(
        Guid id, 
        [FromBody] RedeemLoyaltyPointsRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loyalty = await _customerManagementService.RedeemLoyaltyPointsAsync(id, request.Points, request.Reason, cancellationToken);
            return Ok(loyalty);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Loyalty points redemption failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redeeming loyalty points for customer {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

/// <summary>
/// Request model for adding loyalty points
/// </summary>
public record AddLoyaltyPointsRequest
{
    public int Points { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Request model for redeeming loyalty points
/// </summary>
public record RedeemLoyaltyPointsRequest
{
    public int Points { get; init; }
    public string Reason { get; init; } = string.Empty;
}
