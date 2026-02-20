using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Controller for procurement management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProcurementController : ControllerBase
{
    private readonly IProcurementManagementService _procurementManagementService;
    private readonly ILogger<ProcurementController> _logger;

    public ProcurementController(
        IProcurementManagementService procurementManagementService,
        ILogger<ProcurementController> logger)
    {
        _procurementManagementService = procurementManagementService;
        _logger = logger;
    }

    #region Supplier Management

    /// <summary>
    /// Get supplier by ID
    /// </summary>
    [HttpGet("suppliers/{id}")]
    public async Task<ActionResult<Supplier>> GetSupplier(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var supplier = await _procurementManagementService.GetSupplierByIdAsync(id, cancellationToken);
            if (supplier == null)
            {
                return NotFound($"Supplier with ID {id} not found");
            }

            return Ok(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier with ID {SupplierId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get supplier by code
    /// </summary>
    [HttpGet("suppliers/by-code/{code}")]
    public async Task<ActionResult<Supplier>> GetSupplierByCode(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var supplier = await _procurementManagementService.GetSupplierByCodeAsync(code, cancellationToken);
            if (supplier == null)
            {
                return NotFound($"Supplier with code {code} not found");
            }

            return Ok(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier with code {Code}", code);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get suppliers with pagination
    /// </summary>
    [HttpGet("suppliers")]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var suppliers = await _procurementManagementService.GetSuppliersAsync(page, pageSize, cancellationToken);
            return Ok(suppliers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving suppliers list");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    [HttpPost("suppliers")]
    public async Task<ActionResult<Supplier>> CreateSupplier([FromBody] CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = await _procurementManagementService.CreateSupplierAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Supplier creation failed: {Message}", ex.Message);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing supplier
    /// </summary>
    [HttpPut("suppliers/{id}")]
    public async Task<ActionResult<Supplier>> UpdateSupplier(Guid id, [FromBody] UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = await _procurementManagementService.UpdateSupplierAsync(id, request, cancellationToken);
            return Ok(supplier);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Supplier update failed: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating supplier with ID {SupplierId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a supplier
    /// </summary>
    [HttpDelete("suppliers/{id}")]
    public async Task<ActionResult> DeleteSupplier(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _procurementManagementService.DeleteSupplierAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound($"Supplier with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting supplier with ID {SupplierId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion

    #region Purchase Order Management

    /// <summary>
    /// Get purchase order by ID
    /// </summary>
    [HttpGet("purchase-orders/{id}")]
    public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var purchaseOrder = await _procurementManagementService.GetPurchaseOrderByIdAsync(id, cancellationToken);
            if (purchaseOrder == null)
            {
                return NotFound($"Purchase order with ID {id} not found");
            }

            return Ok(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase order with ID {PurchaseOrderId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get purchase order by order number
    /// </summary>
    [HttpGet("purchase-orders/by-number/{orderNumber}")]
    public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrderByNumber(string orderNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var purchaseOrder = await _procurementManagementService.GetPurchaseOrderByNumberAsync(orderNumber, cancellationToken);
            if (purchaseOrder == null)
            {
                return NotFound($"Purchase order with number {orderNumber} not found");
            }

            return Ok(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase order with number {OrderNumber}", orderNumber);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get purchase orders with pagination
    /// </summary>
    [HttpGet("purchase-orders")]
    public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var purchaseOrders = await _procurementManagementService.GetPurchaseOrdersAsync(page, pageSize, cancellationToken);
            return Ok(purchaseOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase orders list");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get purchase orders by supplier
    /// </summary>
    [HttpGet("purchase-orders/by-supplier/{supplierId}")]
    public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersBySupplier(Guid supplierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var purchaseOrders = await _procurementManagementService.GetPurchaseOrdersBySupplierAsync(supplierId, cancellationToken);
            return Ok(purchaseOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase orders for supplier {SupplierId}", supplierId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get purchase orders by status
    /// </summary>
    [HttpGet("purchase-orders/by-status/{status}")]
    public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersByStatus(string status, CancellationToken cancellationToken = default)
    {
        try
        {
            var purchaseOrders = await _procurementManagementService.GetPurchaseOrdersByStatusAsync(status, cancellationToken);
            return Ok(purchaseOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase orders with status {Status}", status);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new purchase order
    /// </summary>
    [HttpPost("purchase-orders")]
    public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrder = await _procurementManagementService.CreatePurchaseOrderAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = purchaseOrder.Id }, purchaseOrder);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Purchase order creation failed: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing purchase order
    /// </summary>
    [HttpPut("purchase-orders/{id}")]
    public async Task<ActionResult<PurchaseOrder>> UpdatePurchaseOrder(Guid id, [FromBody] UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrder = await _procurementManagementService.UpdatePurchaseOrderAsync(id, request, cancellationToken);
            return Ok(purchaseOrder);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Purchase order update failed: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating purchase order with ID {PurchaseOrderId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a purchase order
    /// </summary>
    [HttpDelete("purchase-orders/{id}")]
    public async Task<ActionResult> DeletePurchaseOrder(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _procurementManagementService.DeletePurchaseOrderAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound($"Purchase order with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting purchase order with ID {PurchaseOrderId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion
}
