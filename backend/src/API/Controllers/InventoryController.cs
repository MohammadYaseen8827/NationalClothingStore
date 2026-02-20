using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Domain.Entities;
using InventoryReport = NationalClothingStore.Application.Services.InventoryReport;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// API controller for inventory management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryManagementService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(
        IInventoryManagementService inventoryService,
        ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Get inventory by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Inventory>> GetInventory(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryAsync(id, cancellationToken);
            if (inventory == null)
            {
                return NotFound();
            }
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory with ID: {InventoryId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get inventory by product
    /// </summary>
    [HttpGet("by-product/{productId}")]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryByProduct(Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByProductAsync(productId, cancellationToken);
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory by product ID: {ProductId}", productId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get inventory by product variation
    /// </summary>
    [HttpGet("by-variation/{productVariationId}")]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryByProductVariation(Guid productVariationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByProductVariationAsync(productVariationId, cancellationToken);
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory by product variation ID: {ProductVariationId}", productVariationId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get inventory by branch
    /// </summary>
    [HttpGet("by-branch/{branchId}")]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryByBranch(Guid branchId, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByBranchAsync(branchId, cancellationToken);
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory by branch ID: {BranchId}", branchId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get inventory by warehouse
    /// </summary>
    [HttpGet("by-warehouse/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryByWarehouse(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByWarehouseAsync(warehouseId, cancellationToken);
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory by warehouse ID: {WarehouseId}", warehouseId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get inventory by location
    /// </summary>
    [HttpGet("by-location")]
    public async Task<ActionResult<Inventory?>> GetInventoryByLocation(
        [FromQuery] Guid productId,
        [FromQuery] Guid? productVariationId,
        [FromQuery] Guid branchId,
        [FromQuery] Guid? warehouseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByLocationAsync(
                productId,
                productVariationId,
                branchId,
                warehouseId,
                cancellationToken);
            if (inventory == null)
            {
                return NotFound();
            }
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory by location");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create new inventory
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Inventory>> CreateInventory(CreateInventoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.CreateInventoryAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for creating inventory");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict when creating inventory");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update existing inventory
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Inventory>> UpdateInventory(Guid id, UpdateInventoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.UpdateInventoryAsync(id, request, cancellationToken);
            return Ok(inventory);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Inventory not found for update: {InventoryId}", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for updating inventory: {InventoryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory: {InventoryId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete inventory
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInventory(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _inventoryService.DeleteInventoryAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Inventory not found for deletion: {InventoryId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inventory: {InventoryId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Reserve inventory
    /// </summary>
    [HttpPost("{id}/reserve")]
    public async Task<ActionResult> ReserveInventory(Guid id, ReserveInventoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _inventoryService.ReserveInventoryAsync(id, request, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Inventory not found for reservation: {InventoryId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot reserve inventory: {InventoryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving inventory: {InventoryId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Release inventory
    /// </summary>
    [HttpPost("{id}/release")]
    public async Task<ActionResult> ReleaseInventory(Guid id, ReleaseInventoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _inventoryService.ReleaseInventoryAsync(id, request, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Inventory not found for release: {InventoryId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot release inventory: {InventoryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing inventory: {InventoryId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update stock quantity
    /// </summary>
    [HttpPut("{id}/stock")]
    public async Task<ActionResult<Inventory>> UpdateStock(
        Guid id, 
        [FromBody] UpdateStockRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.UpdateStockAsync(
                id, 
                request.Quantity, 
                request.UnitCost, 
                request.Reason, 
                request.UserId, 
                cancellationToken);
            return Ok(inventory);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Inventory not found for stock update: {InventoryId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock: {InventoryId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Transfer inventory
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<InventoryTransferResult>> TransferInventory(
        TransferInventoryRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _inventoryService.TransferInventoryAsync(request, cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring inventory");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Bulk transfer inventory
    /// </summary>
    [HttpPost("bulk-transfer")]
    public async Task<ActionResult<InventoryTransferResult>> BulkTransferInventory(
        [FromBody] List<TransferInventoryRequest> requests, 
        [FromQuery] Guid userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _inventoryService.BulkTransferInventoryAsync(requests, userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk transfer inventory");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Adjust inventory
    /// </summary>
    [HttpPut("{id}/adjust")]
    public async Task<ActionResult<Inventory>> AdjustInventory(
        Guid id, 
        AdjustInventoryRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await _inventoryService.AdjustInventoryAsync(id, request, cancellationToken);
            return Ok(inventory);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Inventory not found for adjustment: {InventoryId}", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for adjusting inventory: {InventoryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adjusting inventory: {InventoryId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Bulk adjust inventory
    /// </summary>
    [HttpPut("bulk-adjust")]
    public async Task<ActionResult<IEnumerable<Inventory>>> BulkAdjustInventory(
        BulkInventoryUpdateRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _inventoryService.BulkAdjustInventoryAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk adjust inventory");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Search inventory with pagination
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<PagedResult<Inventory>>> SearchInventory(
        InventorySearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (items, totalCount) = await _inventoryService.SearchInventoryAsync(request, cancellationToken);
            var result = new PagedResult<Inventory>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching inventory");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get low stock items
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetLowStockItems(
        [FromQuery] int threshold = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _inventoryService.GetLowStockItemsAsync(threshold, cancellationToken);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock items");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get out of stock items
    /// </summary>
    [HttpGet("out-of-stock")]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetOutOfStockItems(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _inventoryService.GetOutOfStockItemsAsync(cancellationToken);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting out of stock items");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get inventory statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<InventoryStatistics>> GetInventoryStatistics(
        [FromQuery] Guid? branchId = null, 
        [FromQuery] Guid? warehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _inventoryService.GetInventoryStatisticsAsync(branchId, warehouseId, cancellationToken);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory statistics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get inventory value by location
    /// </summary>
    [HttpGet("value-by-location")]
    public async Task<ActionResult<IEnumerable<LocationInventoryValue>>> GetInventoryValueByLocation(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var values = await _inventoryService.GetInventoryValueByLocationAsync(cancellationToken);
            return Ok(values);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory value by location");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Generate inventory report
    /// </summary>
    [HttpGet("report")]
    public async Task<ActionResult<InventoryReport>> GenerateInventoryReport(
        [FromQuery] Guid? branchId = null, 
        [FromQuery] Guid? warehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _inventoryService.GenerateInventoryReportAsync(branchId, warehouseId, cancellationToken);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating inventory report");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create inventory transaction
    /// </summary>
    [HttpPost("transaction")]
    public async Task<ActionResult<InventoryTransaction>> CreateTransaction(
        CreateTransactionRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _inventoryService.CreateTransactionAsync(
                request.InventoryId,
                request.TransactionType,
                request.Quantity,
                request.UnitCost,
                request.ReferenceNumber,
                request.Reason,
                request.CreatedByUserId,
                request.FromBranchId,
                request.ToBranchId,
                request.FromWarehouseId,
                request.ToWarehouseId,
                cancellationToken);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory transaction");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("transaction/{id}")]
    public async Task<ActionResult<InventoryTransaction>> GetTransaction(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: GetTransactionAsync method not available in interface
            // This endpoint would need to be implemented when the method is added
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory transaction: {TransactionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Search transactions with pagination
    /// </summary>
    [HttpPost("transaction/search")]
    public async Task<ActionResult<PagedResult<InventoryTransaction>>> SearchTransactions(
        InventoryTransactionSearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (items, totalCount) = await _inventoryService.SearchTransactionsAsync(request, cancellationToken);
            var result = new PagedResult<InventoryTransaction>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching inventory transactions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get recent transactions
    /// </summary>
    [HttpGet("transaction/recent")]
    public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetRecentTransactions(
        [FromQuery] int count = 10, 
        [FromQuery] Guid? branchId = null, 
        [FromQuery] Guid? warehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transactions = await _inventoryService.GetRecentTransactionsAsync(count, branchId, warehouseId, cancellationToken);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent inventory transactions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get low stock alerts
    /// </summary>
    [HttpGet("alerts/low-stock")]
    public async Task<ActionResult<IEnumerable<LowStockAlert>>> GetLowStockAlerts(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _inventoryService.GetLowStockAlertsAsync(null, null,cancellationToken);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock alerts");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send low stock alerts
    /// </summary>
    [HttpPost("alerts/low-stock/send")]
    public async Task<ActionResult> SendLowStockAlerts(CancellationToken cancellationToken = default)
    {
        try
        {
            await _inventoryService.SendLowStockAlertsAsync(cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending low stock alerts");
            return StatusCode(500, "Internal server error");
        }
    }
}

// Additional DTOs for API requests
public class UpdateStockRequest
{
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class CreateTransactionRequest
{
    public Guid InventoryId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public Guid? FromBranchId { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
