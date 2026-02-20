namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Sale entity for tracking sales transactions
/// </summary>
public class Sale
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string SaleStatus { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
