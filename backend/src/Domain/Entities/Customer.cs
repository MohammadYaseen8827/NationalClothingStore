using System.ComponentModel.DataAnnotations;

namespace NationalClothingStore.Domain.Entities;

/// <summary>
/// Represents a customer in the clothing store system
/// </summary>
public class Customer
{
    /// <summary>
    /// Unique identifier for the customer
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Customer's first name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's last name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's email address
    /// </summary>
    [StringLength(255)]
    [EmailAddress]
    public string? Email { get; set; }
    
    /// <summary>
    /// Customer's phone number
    /// </summary>
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Customer's date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Customer's gender
    /// </summary>
    [StringLength(20)]
    public string? Gender { get; set; }
    
    /// <summary>
    /// Customer's address line 1
    /// </summary>
    [StringLength(255)]
    public string? AddressLine1 { get; set; }
    
    /// <summary>
    /// Customer's address line 2
    /// </summary>
    [StringLength(255)]
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// Customer's city
    /// </summary>
    [StringLength(100)]
    public string? City { get; set; }
    
    /// <summary>
    /// Customer's state/province
    /// </summary>
    [StringLength(100)]
    public string? State { get; set; }
    
    /// <summary>
    /// Customer's postal code
    /// </summary>
    [StringLength(20)]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Customer's country
    /// </summary>
    [StringLength(100)]
    public string? Country { get; set; }
    
    /// <summary>
    /// Indicates if customer prefers email communications
    /// </summary>
    public bool EmailOptIn { get; set; } = false;
    
    /// <summary>
    /// Indicates if customer prefers SMS communications
    /// </summary>
    public bool SmsOptIn { get; set; } = false;
    
    /// <summary>
    /// Indicates if customer is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when customer was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when customer was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Customer's loyalty program information
    /// </summary>
    public virtual CustomerLoyalty? Loyalty { get; set; }
    
    /// <summary>
    /// Customer's sales transactions
    /// </summary>
    public virtual ICollection<SalesTransaction> SalesTransactions { get; set; } = new List<SalesTransaction>();
    
    // Computed properties
    /// <summary>
    /// Gets the customer's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Gets the customer's age based on DateOfBirth
    /// </summary>
    public int? Age
    {
        get
        {
            if (!DateOfBirth.HasValue) return null;
            
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Value.Year;
            
            // Adjust age if birthday hasn't occurred this year
            if (DateOfBirth.Value.Date > today.AddYears(-age)) 
                age--;
                
            return age;
        }
    }
}