using System.ComponentModel.DataAnnotations;

namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Represents a customer's loyalty program information
/// </summary>
public class CustomerLoyalty
{
    /// <summary>
    /// Unique identifier for the loyalty record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Reference to the customer
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// Customer's loyalty card number
    /// </summary>
    [Required]
    [StringLength(50)]
    public string LoyaltyCardNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Current loyalty points balance
    /// </summary>
    [Range(0, int.MaxValue)]
    public int PointsBalance { get; set; } = 0;
    
    /// <summary>
    /// Total lifetime points earned
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TotalPointsEarned { get; set; } = 0;
    
    /// <summary>
    /// Total points redeemed
    /// </summary>
    [Range(0, int.MaxValue)]
    public int PointsRedeemed { get; set; } = 0;
    
    /// <summary>
    /// Customer's loyalty tier (e.g., Bronze, Silver, Gold, Platinum)
    /// </summary>
    [StringLength(50)]
    public string Tier { get; set; } = "Bronze";
    
    /// <summary>
    /// Points required to reach next tier
    /// </summary>
    [Range(0, int.MaxValue)]
    public int PointsToNextTier { get; set; } = 1000;
    
    /// <summary>
    /// Discount percentage for current tier (0-100)
    /// </summary>
    [Range(0, 100)]
    public decimal TierDiscountPercentage { get; set; } = 0;
    
    /// <summary>
    /// Date when customer joined loyalty program
    /// </summary>
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date of last point activity
    /// </summary>
    public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date of last tier upgrade
    /// </summary>
    public DateTime? LastUpgradeDate { get; set; }
    
    /// <summary>
    /// Indicates if loyalty account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when loyalty record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when loyalty record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Associated customer
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;
    
    /// <summary>
    /// Loyalty point transactions
    /// </summary>
    public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();
    
    // Computed properties
    /// <summary>
    /// Gets the available points that can be redeemed
    /// </summary>
    public int AvailablePoints => Math.Max(0, PointsBalance);
    
    /// <summary>
    /// Gets the progress percentage to next tier (0-100)
    /// </summary>
    public decimal TierProgressPercentage => PointsToNextTier > 0 
        ? Math.Min(100, (decimal)PointsBalance / PointsToNextTier * 100) 
        : 100;
}

/// <summary>
/// Represents a loyalty point transaction
/// </summary>
public class LoyaltyTransaction
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Reference to the customer loyalty record
    /// </summary>
    [Required]
    public Guid CustomerLoyaltyId { get; set; }
    
    /// <summary>
    /// Points amount (positive for earning, negative for redemption)
    /// </summary>
    public int Points { get; set; }
    
    /// <summary>
    /// Type of transaction (EARNED, REDEEMED, EXPIRED, ADJUSTED)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string TransactionType { get; set; } = string.Empty;
    
    /// <summary>
    /// Reason for the transaction
    /// </summary>
    [StringLength(255)]
    public string? Reason { get; set; }
    
    /// <summary>
    /// Reference to associated sales transaction (if applicable)
    /// </summary>
    public Guid? SalesTransactionId { get; set; }
    
    /// <summary>
    /// Date and time of transaction
    /// </summary>
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Expiration date for earned points
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    // Navigation properties
    /// <summary>
    /// Associated customer loyalty record
    /// </summary>
    public virtual CustomerLoyalty CustomerLoyalty { get; set; } = null!;
    
    /// <summary>
    /// Associated sales transaction (if applicable)
    /// </summary>
    public virtual SalesTransaction? SalesTransaction { get; set; }
}