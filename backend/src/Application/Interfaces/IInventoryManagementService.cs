using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Request/response DTOs for inventory management operations
/// </summary>
public record CreateInventoryRequest
{
    public Guid ProductId { get; init; }
    public Guid? ProductVariationId { get; init; }
    public Guid BranchId { get; init; }
    public Guid? WarehouseId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Reason { get; init; } = string.Empty;
    public Guid CreatedByUserId { get; init; }
}

public record UpdateInventoryRequest
{
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Reason { get; init; } = string.Empty;
    public Guid UpdatedByUserId { get; init; }
}

public record ReserveInventoryRequest
{
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public Guid ReservedByUserId { get; init; }
}

public record ReleaseInventoryRequest
{
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public Guid ReleasedByUserId { get; init; }
}

public record TransferInventoryRequest
{
    public Guid FromInventoryId { get; init; }
    public Guid ToBranchId { get; init; }
    public Guid? ToWarehouseId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Reason { get; init; } = string.Empty;
    public Guid TransferredByUserId { get; init; }
}

public record AdjustInventoryRequest
{
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Reason { get; init; } = string.Empty;
    public Guid AdjustedByUserId { get; init; }
}

public record BulkInventoryUpdateRequest
{
    public List<UpdateInventoryItem> Items { get; init; } = new();
    public string Reason { get; init; } = string.Empty;
    public Guid UpdatedByUserId { get; init; }
}

public record UpdateInventoryItem
{
    public Guid InventoryId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
}

public record InventorySearchRequest
{
    public string? SearchTerm { get; init; }
    public Guid? BranchId { get; init; }
    public Guid? WarehouseId { get; init; }
    public Guid? ProductId { get; init; }
    public Guid? ProductVariationId { get; init; }
    public bool? LowStock { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}

public record InventoryTransactionSearchRequest
{
    public Guid? InventoryId { get; init; }
    public Guid? ProductId { get; init; }
    public Guid? ProductVariationId { get; init; }
    public Guid? BranchId { get; init; }
    public Guid? WarehouseId { get; init; }
    public string? TransactionType { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public Guid? UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Service interface for inventory management operations
/// </summary>
public interface IInventoryManagementService
{
    // Inventory CRUD operations
    Task<Inventory> CreateInventoryAsync(CreateInventoryRequest request, CancellationToken cancellationToken = default);
    Task<Inventory?> GetInventoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Inventory> UpdateInventoryAsync(Guid id, UpdateInventoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteInventoryAsync(Guid id, CancellationToken cancellationToken = default);

    // Inventory location operations
    Task<Inventory?> GetInventoryByLocationAsync(
        Guid productId, 
        Guid? productVariationId, 
        Guid branchId, 
        Guid? warehouseId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Inventory>> GetInventoryByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetInventoryByProductVariationAsync(Guid productVariationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetInventoryByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetInventoryByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);

    // Stock management operations
    Task ReserveInventoryAsync(Guid inventoryId, ReserveInventoryRequest request, CancellationToken cancellationToken = default);
    Task ReleaseInventoryAsync(Guid inventoryId, ReleaseInventoryRequest request, CancellationToken cancellationToken = default);
    Task<Inventory> UpdateStockAsync(Guid inventoryId, int quantity, decimal unitCost, string reason, Guid userId, CancellationToken cancellationToken = default);

    // Transfer operations
    Task<InventoryTransferResult> TransferInventoryAsync(TransferInventoryRequest request, CancellationToken cancellationToken = default);
    Task<InventoryTransferResult> BulkTransferInventoryAsync(List<TransferInventoryRequest> requests, Guid userId, CancellationToken cancellationToken = default);

    // Adjustment operations
    Task<Inventory> AdjustInventoryAsync(Guid inventoryId, AdjustInventoryRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> BulkAdjustInventoryAsync(BulkInventoryUpdateRequest request, CancellationToken cancellationToken = default);

    // Search and filtering
    Task<(IEnumerable<Inventory> items, int totalCount)> SearchInventoryAsync(InventorySearchRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync(CancellationToken cancellationToken = default);

    // Statistics and reporting
    Task<InventoryStatistics> GetInventoryStatisticsAsync(
        Guid? branchId = null, 
        Guid? warehouseId = null, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<LocationInventoryValue>> GetInventoryValueByLocationAsync(CancellationToken cancellationToken = default);
    Task<InventoryReport> GenerateInventoryReportAsync(
        Guid? branchId = null, 
        Guid? warehouseId = null, 
        CancellationToken cancellationToken = default);

    // Transaction management
    Task<InventoryTransaction> CreateTransactionAsync(
        Guid inventoryId, 
        string transactionType, 
        int quantity, 
        decimal unitCost, 
        string referenceNumber, 
        string reason, 
        Guid createdByUserId, 
        Guid? fromBranchId = null, 
        Guid? toBranchId = null, 
        Guid? fromWarehouseId = null, 
        Guid? toWarehouseId = null, 
        CancellationToken cancellationToken = default);

    Task<(IEnumerable<InventoryTransaction> items, int totalCount)> SearchTransactionsAsync(InventoryTransactionSearchRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryTransaction>> GetRecentTransactionsAsync(
        int count = 10, 
        Guid? branchId = null, 
        Guid? warehouseId = null, 
        CancellationToken cancellationToken = default);

    // Validation
    Task<ValidationResult> ValidateInventoryRequest(CreateInventoryRequest request, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateTransferRequest(TransferInventoryRequest request, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateAdjustmentRequest(Guid inventoryId, AdjustInventoryRequest request, CancellationToken cancellationToken = default);

    // Low stock alerts
    Task<IEnumerable<LowStockAlert>> GetLowStockAlertsAsync(Guid? branchId = null , Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task SendLowStockAlertsAsync(CancellationToken cancellationToken = default);

    // Summary and reporting
    Task<object> GetInventorySummaryAsync(Guid? branchId = null, Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetRecentMovementsAsync(DateTime startDate, DateTime endDate, Guid? branchId = null, Guid? warehouseId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of inventory transfer operation
/// </summary>
public record InventoryTransferResult
{
    public bool Success { get; init; }
    public Guid FromInventoryId { get; init; }
    public Guid ToInventoryId { get; init; }
    public int TransferredQuantity { get; init; }
    public decimal TransferredValue { get; init; }
    public string Message { get; init; } = string.Empty;
    public List<string> Warnings { get; init; } = new();
    public List<InventoryTransaction> Transactions { get; init; } = new();
}

/// <summary>
/// Low stock alert
/// </summary>
public record LowStockAlert
{
    public Guid InventoryId { get; init; }
    public Guid ProductId { get; init; }
    public Guid? ProductVariationId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSKU { get; init; } = string.Empty;
    public string? ProductVariationSize { get; init; }
    public string? ProductVariationColor { get; init; }
    public Guid BranchId { get; init; }
    public Guid? WarehouseId { get; init; }
    public string LocationName { get; init; } = string.Empty;
    public int CurrentQuantity { get; init; }
    public int LowStockThreshold { get; init; }
    public int ReorderPoint { get; init; }
    public decimal UnitCost { get; init; }
    public decimal TotalValue { get; init; }
    public DateTime AlertDate { get; init; }
    public bool IsResolved { get; init; }
    public DateTime? ResolvedDate { get; init; }
}


