

using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.Infrastructure.External;

/// <summary>
/// Mock payment service for processing various payment methods
/// In production, this would integrate with actual payment processors
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly Random _random = new();

    public PaymentService(ILogger<PaymentService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Process a credit card payment
    /// </summary>
    /// <param name="request">Credit card payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    public async Task<PaymentResult> ProcessCreditCardPaymentAsync(
        CreditCardPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing credit card payment for amount {Amount} {Currency}", 
            request.Amount, request.Currency);

        // Simulate payment processing delay
        await Task.Delay(_random.Next(500, 2000), cancellationToken);

        // Mock approval logic (95% approval rate)
        var isApproved = _random.NextDouble() > 0.05;
        var authorizationCode = isApproved ? GenerateAuthorizationCode() : null;

        var result = new PaymentResult
        {
            IsApproved = isApproved,
            AuthorizationCode = authorizationCode,
            TransactionId = isApproved ? Guid.NewGuid().ToString("N")[..16] : null,
            CardLastFour = request.CardNumber.Substring(Math.Max(0, request.CardNumber.Length - 4)),
            CardType = GetCardType(request.CardNumber),
            ErrorMessage = isApproved ? null : "Payment declined by bank"
        };

        _logger.LogInformation("Credit card payment {Status} - Auth Code: {AuthCode}", 
            isApproved ? "approved" : "declined", authorizationCode);

        return result;
    }

    /// <summary>
    /// Process a debit card payment
    /// </summary>
    /// <param name="request">Debit card payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    public async Task<PaymentResult> ProcessDebitCardPaymentAsync(
        DebitCardPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing debit card payment for amount {Amount} {Currency}", 
            request.Amount, request.Currency);

        // Simulate payment processing delay
        await Task.Delay(_random.Next(300, 1500), cancellationToken);

        // Mock approval logic (98% approval rate for debit)
        var isApproved = _random.NextDouble() > 0.02;
        var authorizationCode = isApproved ? GenerateAuthorizationCode() : null;

        var result = new PaymentResult
        {
            IsApproved = isApproved,
            AuthorizationCode = authorizationCode,
            TransactionId = isApproved ? Guid.NewGuid().ToString("N")[..16] : null,
            CardLastFour = request.CardNumber.Substring(Math.Max(0, request.CardNumber.Length - 4)),
            CardType = GetCardType(request.CardNumber),
            ErrorMessage = isApproved ? null : "Insufficient funds or card declined"
        };

        _logger.LogInformation("Debit card payment {Status} - Auth Code: {AuthCode}", 
            isApproved ? "approved" : "declined", authorizationCode);

        return result;
    }

    /// <summary>
    /// Process a cash payment
    /// </summary>
    /// <param name="request">Cash payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    public async Task<PaymentResult> ProcessCashPaymentAsync(
        CashPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing cash payment for amount {Amount} {Currency}", 
            request.Amount, request.Currency);

        // Cash payments are always approved
        await Task.Delay(100, cancellationToken); // Minimal delay

        var result = new PaymentResult
        {
            IsApproved = true,
            AuthorizationCode = "CASH-" + Guid.NewGuid().ToString("N")[..8],
            TransactionId = Guid.NewGuid().ToString("N")[..16],
            ErrorMessage = null
        };

        _logger.LogInformation("Cash payment approved - Transaction ID: {TransactionId}", 
            result.TransactionId);

        return result;
    }

    /// <summary>
    /// Process a gift card payment
    /// </summary>
    /// <param name="request">Gift card payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    public async Task<PaymentResult> ProcessGiftCardPaymentAsync(
        GiftCardPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing gift card payment for card {CardNumber}", 
            MaskGiftCardNumber(request.GiftCardNumber));

        // Simulate gift card balance check
        await Task.Delay(_random.Next(200, 800), cancellationToken);

        // Mock balance check (80% have sufficient balance)
        var hasSufficientBalance = _random.NextDouble() > 0.20;
        
        var result = new PaymentResult
        {
            IsApproved = hasSufficientBalance,
            AuthorizationCode = hasSufficientBalance ? GenerateAuthorizationCode() : null,
            TransactionId = hasSufficientBalance ? Guid.NewGuid().ToString("N")[..16] : null,
            GiftCardNumber = MaskGiftCardNumber(request.GiftCardNumber),
            ErrorMessage = hasSufficientBalance ? null : "Insufficient balance on gift card"
        };

        _logger.LogInformation("Gift card payment {Status} - Auth Code: {AuthCode}", 
            hasSufficientBalance ? "approved" : "declined", result.AuthorizationCode);

        return result;
    }

    /// <summary>
    /// Validate credit card information
    /// </summary>
    /// <param name="cardNumber">Card number</param>
    /// <param name="expiryMonth">Expiry month</param>
    /// <param name="expiryYear">Expiry year</param>
    /// <param name="cvv">CVV code</param>
    /// <returns>Validation result</returns>
    public Application.Interfaces.CardValidationResult ValidateCard(string cardNumber, int expiryMonth, int expiryYear, string cvv)
    {
        var errors = new List<string>();

        // Basic format validation
        if (string.IsNullOrWhiteSpace(cardNumber) || !IsValidLuhn(cardNumber.Replace(" ", "")))
        {
            errors.Add("Invalid card number");
        }

        if (expiryMonth < 1 || expiryMonth > 12)
        {
            errors.Add("Invalid expiry month");
        }

        var currentDate = DateTime.UtcNow;
        var expiryDate = new DateTime(expiryYear, expiryMonth, 1).AddMonths(1).AddDays(-1);
        
        if (expiryDate < currentDate)
        {
            errors.Add("Card has expired");
        }

        if (string.IsNullOrWhiteSpace(cvv) || cvv.Length < 3 || cvv.Length > 4)
        {
            errors.Add("Invalid CVV");
        }

        var isValid = errors.Count == 0;
        var cardType = isValid ? GetCardType(cardNumber) : null;

        return new Application.Interfaces.CardValidationResult
        {
            IsValid = isValid,
            CardType = cardType,
            Errors = errors
        };
    }

    /// <summary>
    /// Generate a mock authorization code
    /// </summary>
    /// <returns>Authorization code</returns>
    private string GenerateAuthorizationCode()
    {
        return "AUTH" + _random.Next(100000, 999999);
    }

    /// <summary>
    /// Determine card type from card number
    /// </summary>
    /// <param name="cardNumber">Card number</param>
    /// <returns>Card type</returns>
    private string GetCardType(string cardNumber)
    {
        var cleanNumber = cardNumber.Replace(" ", "").Replace("-", "");
        
        if (cleanNumber.StartsWith("4")) return "Visa";
        if (cleanNumber.StartsWith("5") || cleanNumber.StartsWith("2")) return "MasterCard";
        if (cleanNumber.StartsWith("34") || cleanNumber.StartsWith("37")) return "American Express";
        if (cleanNumber.StartsWith("6")) return "Discover";
        
        return "Unknown";
    }

    /// <summary>
    /// Mask gift card number for security
    /// </summary>
    /// <param name="giftCardNumber">Gift card number</param>
    /// <returns>Masked number</returns>
    private string MaskGiftCardNumber(string giftCardNumber)
    {
        if (string.IsNullOrEmpty(giftCardNumber)) return string.Empty;
        
        var last4 = giftCardNumber.Substring(Math.Max(0, giftCardNumber.Length - 4));
        return $"****-****-****-{last4}";
    }

    /// <summary>
    /// Validate credit card number using Luhn algorithm
    /// </summary>
    /// <param name="cardNumber">Card number</param>
    /// <returns>True if valid</returns>
    private bool IsValidLuhn(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber)) return false;

        var sum = 0;
        var alternate = false;
        
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(cardNumber[i])) return false;
            
            var digit = int.Parse(cardNumber[i].ToString());
            
            if (alternate)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            
            sum += digit;
            alternate = !alternate;
        }
        
        return sum % 10 == 0;
    }
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