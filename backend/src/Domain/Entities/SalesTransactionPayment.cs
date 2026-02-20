using System.ComponentModel.DataAnnotations;

namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Represents a payment method used in a sales transaction
/// </summary>
public class SalesTransactionPayment
{
    /// <summary>
    /// Unique identifier for the payment
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Reference to the sales transaction
    /// </summary>
    [Required]
    public Guid SalesTransactionId { get; set; }
    
    /// <summary>
    /// Payment method type (CASH, CREDIT_CARD, DEBIT_CARD, MOBILE_PAYMENT, GIFT_CARD)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;
    
    /// <summary>
    /// Payment amount
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; } = 0;
    
    /// <summary>
    /// Currency code (e.g., USD, EUR)
    /// </summary>
    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Payment reference number (e.g., credit card transaction ID)
    /// </summary>
    [StringLength(100)]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Credit card last 4 digits (if applicable)
    /// </summary>
    [StringLength(4)]
    public string? CardLastFour { get; set; }
    
    /// <summary>
    /// Credit card type (VISA, MASTERCARD, AMEX, etc.)
    /// </summary>
    [StringLength(20)]
    public string? CardType { get; set; }
    
    /// <summary>
    /// Gift card number (if applicable)
    /// </summary>
    [StringLength(50)]
    public string? GiftCardNumber { get; set; }
    
    /// <summary>
    /// Authorization code from payment processor
    /// </summary>
    [StringLength(100)]
    public string? AuthorizationCode { get; set; }
    
    /// <summary>
    /// Indicates if payment was approved
    /// </summary>
    public bool IsApproved { get; set; } = true;
    
    /// <summary>
    /// Error message if payment was declined
    /// </summary>
    [StringLength(500)]
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Date and time when payment was processed
    /// </summary>
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Parent sales transaction
    /// </summary>
    public virtual SalesTransaction SalesTransaction { get; set; } = null!;
}