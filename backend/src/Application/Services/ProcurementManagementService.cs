using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Application.Services;

/// <summary>
/// Service for managing procurement operations and supplier relationships
/// </summary>
public class ProcurementManagementService : IProcurementManagementService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ILogger<ProcurementManagementService> _logger;

    public ProcurementManagementService(
        ISupplierRepository supplierRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        ILogger<ProcurementManagementService> logger)
    {
        _supplierRepository = supplierRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _logger = logger;
    }

    public async Task<Supplier?> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _supplierRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier with ID {SupplierId}", id);
            throw;
        }
    }

    public async Task<Supplier?> GetSupplierByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _supplierRepository.GetByCodeAsync(code, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier with code {Code}", code);
            throw;
        }
    }

    public async Task<IEnumerable<Supplier>> GetSuppliersAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var (suppliers, _) = await _supplierRepository.GetPagedAsync(page, pageSize, cancellationToken: cancellationToken);
            return suppliers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving suppliers list");
            throw;
        }
    }

    public async Task<Supplier> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if supplier with code already exists
            var existingSupplier = await _supplierRepository.GetByCodeAsync(request.Code, cancellationToken);
            if (existingSupplier != null)
            {
                throw new InvalidOperationException($"Supplier with code {request.Code} already exists");
            }

            var supplier = new Supplier
            {
                Name = request.Name,
                Code = request.Code,
                ContactPerson = request.ContactPerson,
                Email = request.Email,
                Phone = request.Phone,
                Address = $"{request.AddressLine1} {request.AddressLine2}".Trim(),
                City = request.City,
                Country = request.Country,
                TaxNumber = request.TaxId,
                PaymentTerms = request.PaymentTerms,
                Website = request.Website,
                Rating = request.Rating,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdSupplier = await _supplierRepository.CreateAsync(supplier, cancellationToken);
            _logger.LogInformation("Created new supplier with ID {SupplierId}", createdSupplier.Id);

            return createdSupplier;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier with code {Code}", request.Code);
            throw;
        }
    }

    public async Task<Supplier> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {id} not found");
            }

            supplier.Name = request.Name ?? supplier.Name;
            supplier.ContactPerson = request.ContactPerson ?? supplier.ContactPerson;
            supplier.Email = request.Email ?? supplier.Email;
            supplier.Phone = request.Phone ?? supplier.Phone;
            supplier.Address = request.Address ?? supplier.Address;
            supplier.City = request.City ?? supplier.City;
            supplier.Country = request.Country ?? supplier.Country;
            supplier.TaxNumber = request.TaxId ?? supplier.TaxNumber;
            supplier.PaymentTerms = request.PaymentTerms ?? supplier.PaymentTerms;
            supplier.Website = request.Website ?? supplier.Website;
            supplier.Rating = request.Rating ?? supplier.Rating;
            supplier.Notes = request.Notes ?? supplier.Notes;
            supplier.UpdatedAt = DateTime.UtcNow;

            var updatedSupplier = await _supplierRepository.UpdateAsync(supplier, cancellationToken);
            _logger.LogInformation("Updated supplier with ID {SupplierId}", id);

            return updatedSupplier;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating supplier with ID {SupplierId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _supplierRepository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.LogInformation("Deleted supplier with ID {SupplierId}", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting supplier with ID {SupplierId}", id);
            throw;
        }
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase order with ID {PurchaseOrderId}", id);
            throw;
        }
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _purchaseOrderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase order with number {OrderNumber}", orderNumber);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var (orders, _) = await _purchaseOrderRepository.GetPagedAsync(page, pageSize, cancellationToken: cancellationToken);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase orders list");
            throw;
        }
    }

    public async Task<PurchaseOrder> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {request.SupplierId} not found");
            }

            var purchaseOrder = new PurchaseOrder
            {
                SupplierId = request.SupplierId,
                OrderDate = request.OrderDate ?? DateTime.UtcNow,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                Status = "Pending",
                TotalAmount = request.Items.Sum(item => item.Quantity * item.UnitPrice),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdOrder = await _purchaseOrderRepository.CreateAsync(purchaseOrder, cancellationToken);
            
            // Create purchase order items
            foreach (var itemRequest in request.Items)
            {
                var item = new PurchaseOrderItem
                {
                    PurchaseOrderId = createdOrder.Id,
                    ProductId = itemRequest.ProductId,
                    ProductVariationId = itemRequest.ProductVariationId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.UnitPrice,
                    DiscountAmount = itemRequest.DiscountAmount,
                    TotalPrice = itemRequest.Quantity * itemRequest.UnitPrice - itemRequest.DiscountAmount,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Add item to database (this would need a separate repository for items)
                // For now, we'll add them through the order context
            }

            _logger.LogInformation("Created new purchase order with ID {PurchaseOrderId}", createdOrder.Id);
            return createdOrder;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            throw;
        }
    }

    public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken);
            if (purchaseOrder == null)
            {
                throw new KeyNotFoundException($"Purchase order with ID {id} not found");
            }

            purchaseOrder.Status = request.Status ?? purchaseOrder.Status;
            purchaseOrder.ExpectedDeliveryDate = request.ExpectedDeliveryDate ?? purchaseOrder.ExpectedDeliveryDate;
            purchaseOrder.Notes = request.Notes ?? purchaseOrder.Notes;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            var updatedOrder = await _purchaseOrderRepository.UpdateAsync(purchaseOrder, cancellationToken);
            _logger.LogInformation("Updated purchase order with ID {PurchaseOrderId}", id);

            return updatedOrder;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating purchase order with ID {PurchaseOrderId}", id);
            throw;
        }
    }

    public async Task<bool> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _purchaseOrderRepository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.LogInformation("Deleted purchase order with ID {PurchaseOrderId}", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting purchase order with ID {PurchaseOrderId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _purchaseOrderRepository.GetBySupplierAsync(supplierId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase orders for supplier {SupplierId}", supplierId);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _purchaseOrderRepository.GetByStatusAsync(status, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase orders with status {Status}", status);
            throw;
        }
    }
}

/// <summary>
/// Request model for creating a supplier
/// </summary>
public record CreateSupplierRequest
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string AddressLine2 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string TaxId { get; init; } = string.Empty;
    public string PaymentTerms { get; init; } = string.Empty;
    public string Website { get; init; } = string.Empty;
    public string Rating { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

/// <summary>
/// Request model for updating a supplier
/// </summary>
public record UpdateSupplierRequest
{
    public string? Name { get; init; }
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? TaxId { get; init; }
    public string? PaymentTerms { get; init; }
    public string? Website { get; init; }
    public string? Rating { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Request model for creating a purchase order
/// </summary>
public record CreatePurchaseOrderRequest
{
    public Guid SupplierId { get; init; }
    public DateTime? OrderDate { get; init; }
    public DateTime? ExpectedDeliveryDate { get; init; }
    public List<CreatePurchaseOrderItemRequest> Items { get; init; } = new();
}

/// <summary>
/// Request model for creating a purchase order item
/// </summary>
public record CreatePurchaseOrderItemRequest
{
    public Guid ProductId { get; init; }
    public Guid? ProductVariationId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountAmount { get; init; } = 0;
}

/// <summary>
/// Request model for updating a purchase order
/// </summary>
public record UpdatePurchaseOrderRequest
{
    public string? Status { get; init; }
    public DateTime? ExpectedDeliveryDate { get; init; }
    public string? Notes { get; init; }
}
