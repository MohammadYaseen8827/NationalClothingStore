using NationalClothingStore.Application.Services;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Service interface for procurement management operations
/// </summary>
public interface IProcurementManagementService
{
    /// <summary>
    /// Get supplier by ID
    /// </summary>
    Task<Supplier?> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get supplier by code
    /// </summary>
    Task<Supplier?> GetSupplierByCodeAsync(string code, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get suppliers with pagination
    /// </summary>
    Task<IEnumerable<Supplier>> GetSuppliersAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new supplier
    /// </summary>
    Task<Supplier> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing supplier
    /// </summary>
    Task<Supplier> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a supplier
    /// </summary>
    Task<bool> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase order by ID
    /// </summary>
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase order by order number
    /// </summary>
    Task<PurchaseOrder?> GetPurchaseOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase orders with pagination
    /// </summary>
    Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new purchase order
    /// </summary>
    Task<PurchaseOrder> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing purchase order
    /// </summary>
    Task<PurchaseOrder> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a purchase order
    /// </summary>
    Task<bool> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase orders by supplier
    /// </summary>
    Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get purchase orders by status
    /// </summary>
    Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status, CancellationToken cancellationToken = default);
}
