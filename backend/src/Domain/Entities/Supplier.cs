namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Supplier entity for supplier management
/// </summary>
public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string PaymentTerms { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    public string Rating { get; set; } = string.Empty;
    
    public string Notes { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}
