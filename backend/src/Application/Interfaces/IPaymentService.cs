using System.Threading;
using System.Threading.Tasks;

namespace NationalClothingStore.Application.Interfaces;

/// <summary>
/// Interface for payment processing services
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Process a credit card payment
    /// </summary>
    /// <param name="request">Credit card payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    Task<PaymentResult> ProcessCreditCardPaymentAsync(
        CreditCardPaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Process a debit card payment
    /// </summary>
    /// <param name="request">Debit card payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    Task<PaymentResult> ProcessDebitCardPaymentAsync(
        DebitCardPaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Process a cash payment
    /// </summary>
    /// <param name="request">Cash payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    Task<PaymentResult> ProcessCashPaymentAsync(
        CashPaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Process a gift card payment
    /// </summary>
    /// <param name="request">Gift card payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    Task<PaymentResult> ProcessGiftCardPaymentAsync(
        GiftCardPaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate credit card information
    /// </summary>
    /// <param name="cardNumber">Card number</param>
    /// <param name="expiryMonth">Expiry month</param>
    /// <param name="expiryYear">Expiry year</param>
    /// <param name="cvv">CVV code</param>
    /// <returns>Validation result</returns>
    CardValidationResult ValidateCard(string cardNumber, int expiryMonth, int expiryYear, string cvv);
}

/// <summary>
/// Credit card payment request
/// </summary>
public class CreditCardPaymentRequest
{
    public string CardNumber { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Cvv { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string CardholderName { get; set; } = string.Empty;
}

/// <summary>
/// Debit card payment request
/// </summary>
public class DebitCardPaymentRequest
{
    public string CardNumber { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Pin { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// Cash payment request
/// </summary>
public class CashPaymentRequest
{
    public decimal Amount { get; set; }
    public decimal CashTendered { get; set; }
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// Gift card payment request
/// </summary>
public class GiftCardPaymentRequest
{
    public string GiftCardNumber { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// Payment result
/// </summary>
public class PaymentResult
{
    public bool IsApproved { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? TransactionId { get; set; }
    public string? CardLastFour { get; set; }
    public string? CardType { get; set; }
    public string? GiftCardNumber { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Card validation result
/// </summary>
public class CardValidationResult
{
    public bool IsValid { get; set; }
    public string? CardType { get; set; }
    public List<string> Errors { get; set; } = new();
}