namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// PurchaseOrder entity for supplier orders
/// </summary>
public class PurchaseOrder
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty; // DRAFT, SENT, RECEIVED, CANCELLED
    public string PaymentStatus { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    
    public DateTime? ReceivedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
