using System.ComponentModel.DataAnnotations;

namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Represents a sales transaction in the clothing store
/// </summary>
public class SalesTransaction
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Transaction number/identifier
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TransactionNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference to the branch where transaction occurred
    /// </summary>
    [Required]
    public Guid BranchId { get; set; }
    
    /// <summary>
    /// Reference to the customer (nullable for walk-in customers)
    /// </summary>
    public Guid? CustomerId { get; set; }
    
    /// <summary>
    /// Reference to the user who processed the transaction
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Transaction type (SALE, RETURN, EXCHANGE)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string TransactionType { get; set; } = "SALE";
    
    /// <summary>
    /// Transaction status (PENDING, COMPLETED, CANCELLED, REFUNDED)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "COMPLETED";
    
    /// <summary>
    /// Original transaction ID (for returns/exchanges)
    /// </summary>
    public Guid? OriginalTransactionId { get; set; }
    
    /// <summary>
    /// Subtotal amount before tax and discounts
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Subtotal { get; set; } = 0;
    
    /// <summary>
    /// Tax amount
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; } = 0;
    
    /// <summary>
    /// Discount amount
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; } = 0;
    
    /// <summary>
    /// Total transaction amount
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; } = 0;
    
    /// <summary>
    /// Amount paid by customer
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AmountPaid { get; set; } = 0;
    
    /// <summary>
    /// Change given to customer
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal ChangeGiven { get; set; } = 0;
    
    /// <summary>
    /// Loyalty points earned in this transaction
    /// </summary>
    [Range(0, int.MaxValue)]
    public int LoyaltyPointsEarned { get; set; } = 0;
    
    /// <summary>
    /// Loyalty points redeemed in this transaction
    /// </summary>
    [Range(0, int.MaxValue)]
    public int LoyaltyPointsRedeemed { get; set; } = 0;
    
    /// <summary>
    /// Notes about the transaction
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Date and time when transaction was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when transaction was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when transaction was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Branch where transaction occurred
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;
    
    /// <summary>
    /// Customer associated with transaction
    /// </summary>
    public virtual Customer? Customer { get; set; }
    
    /// <summary>
    /// User who processed the transaction
    /// </summary>
    public virtual User User { get; set; } = null!;
    
    /// <summary>
    /// Original transaction (for returns/exchanges)
    /// </summary>
    public virtual SalesTransaction? OriginalTransaction { get; set; }
    
    /// <summary>
    /// Return/exchange transactions
    /// </summary>
    public virtual ICollection<SalesTransaction> ReturnTransactions { get; set; } = new List<SalesTransaction>();
    
    /// <summary>
    /// Items in the transaction
    /// </summary>
    public virtual ICollection<SalesTransactionItem> Items { get; set; } = new List<SalesTransactionItem>();
    
    /// <summary>
    /// Payment methods used
    /// </summary>
    public virtual ICollection<SalesTransactionPayment> Payments { get; set; } = new List<SalesTransactionPayment>();
    
    /// <summary>
    /// Loyalty transaction (if points were earned/redeemed)
    /// </summary>
    public virtual IEnumerable<LoyaltyTransaction>? LoyaltyTransaction { get; set; }
    
    // Computed properties
    /// <summary>
    /// Gets the net amount (total minus discount)
    /// </summary>
    public decimal NetAmount => Math.Max(0, TotalAmount - DiscountAmount);
    
    /// <summary>
    /// Gets the outstanding balance
    /// </summary>
    public decimal OutstandingBalance => Math.Max(0, NetAmount - AmountPaid);
    
    /// <summary>
    /// Gets whether the transaction is fully paid
    /// </summary>
    public bool IsFullyPaid => OutstandingBalance == 0;
    
    /// <summary>
    /// Gets the number of items in the transaction
    /// </summary>
    public int ItemCount => Items.Sum(item => item.Quantity);
}