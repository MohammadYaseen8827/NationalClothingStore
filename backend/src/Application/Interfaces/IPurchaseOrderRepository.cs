using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Repository interface for PurchaseOrder entity operations
/// </summary>
public interface IPurchaseOrderRepository
{
    /// <summary>
    /// Get purchase order by ID with navigation properties
    /// </summary>
    Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase order by order number
    /// </summary>
    Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all purchase orders
    /// </summary>
    Task<IEnumerable<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase orders by supplier
    /// </summary>
    Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase orders by status
    /// </summary>
    Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase orders with pagination and filtering
    /// </summary>
    Task<(IEnumerable<PurchaseOrder> orders, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        string? status = null,
        Guid? supplierId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new purchase order
    /// </summary>
    Task<PurchaseOrder> CreateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing purchase order
    /// </summary>
    Task<PurchaseOrder> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a purchase order
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if purchase order exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if order number already exists
    /// </summary>
    Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase order count
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase order count by status
    /// </summary>
    Task<int> GetCountByStatusAsync(string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get total value of purchase orders
    /// </summary>
    Task<decimal> GetTotalValueAsync(CancellationToken cancellationToken = default);
}
