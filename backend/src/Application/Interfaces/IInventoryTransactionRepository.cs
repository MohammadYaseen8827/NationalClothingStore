using NationalClothingStore.Domain.Entities;
using NationalClothingStore.Application.Common;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for InventoryTransaction entity
/// </summary>
public interface IInventoryTransactionRepository : IRepository<InventoryTransaction>
{
    /// <summary>
    /// Get inventory transaction by ID with related entities
    /// </summary>
    new Task<InventoryTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for a specific inventory item
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for a specific product across all locations
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for a specific product variation across all locations
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByProductVariationIdAsync(Guid productVariationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for a specific branch
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for a specific warehouse
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by type
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByTransactionTypeAsync(string transactionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by date range
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by user
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions with pagination and filtering
    /// </summary>
    Task<(IEnumerable<InventoryTransaction> items, int totalCount)> GetWithPaginationAsync(
        int page,
        int pageSize,
        Guid? inventoryId = null,
        Guid? productId = null,
        Guid? productVariationId = null,
        Guid? branchId = null,
        Guid? warehouseId = null,
        string? transactionType = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction summary by type
    /// </summary>
    Task<IEnumerable<TransactionSummary>> GetTransactionSummaryAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recent transactions
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetRecentTransactionsAsync(
        int count = 10,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get movements (transactions) by date range and location
    /// </summary>
    Task<IEnumerable<InventoryMovement>> GetMovementsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by reference number
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transfer transactions
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetTransferTransactionsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get adjustment transactions
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetAdjustmentTransactionsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction statistics
    /// </summary>
    Task<TransactionStatistics> GetStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? branchId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
}
