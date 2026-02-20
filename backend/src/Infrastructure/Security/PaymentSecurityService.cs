using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using NationalClothingStore.Application.Interfaces;

namespace NationalClothingStore.Infrastructure.Security;

/// <summary>
/// Security service for payment processing with PCI DSS compliance
/// </summary>
public class PaymentSecurityService : IPaymentSecurityService
{
    private readonly ILogger<PaymentSecurityService> _logger;
    private readonly byte[] _encryptionKey;
    private readonly byte[] _iv;

    public PaymentSecurityService(ILogger<PaymentSecurityService> logger)
    {
        _logger = logger;
        
        // In production, these should be stored securely (Azure Key Vault, AWS KMS, etc.)
        // For demonstration, using fixed keys - NEVER do this in production!
        _encryptionKey = Encoding.UTF8.GetBytes("ThisIsA32ByteLongEncryptionKey!!");
        _iv = Encoding.UTF8.GetBytes("16ByteIV!!"); // Must be 16 bytes for AES
    }

    /// <summary>
    /// Encrypt sensitive payment data
    /// </summary>
    public string EncryptPaymentData(string plainText)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            aes.IV = _iv;
            
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(plainText);
            }
            
            var encrypted = ms.ToArray();
            return Convert.ToBase64String(encrypted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting payment data");
            throw new SecurityException("Failed to encrypt payment data", ex);
        }
    }

    /// <summary>
    /// Decrypt sensitive payment data
    /// </summary>
    public string DecryptPaymentData(string encryptedText)
    {
        try
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            
            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            aes.IV = _iv;
            
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(encryptedBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);
            
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting payment data");
            throw new SecurityException("Failed to decrypt payment data", ex);
        }
    }

    /// <summary>
    /// Mask credit card number for display
    /// </summary>
    public string MaskCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return string.Empty;

        var cleanNumber = cardNumber.Replace(" ", "").Replace("-", "");
        
        if (cleanNumber.Length < 4)
            return cleanNumber;

        var last4 = cleanNumber.Substring(cleanNumber.Length - 4);
        var masked = new string('*', Math.Min(cleanNumber.Length - 4, 12));
        
        return $"{masked}{last4}";
    }

    /// <summary>
    /// Validate PCI DSS compliant card data handling
    /// </summary>
    public bool ValidatePCIDataHandling(string cardNumber, string cvv, DateTime expiryDate)
    {
        var violations = new List<string>();

        // Check if card data is being stored (should not store full card numbers)
        if (!string.IsNullOrWhiteSpace(cardNumber) && cardNumber.Length > 16)
        {
            violations.Add("Full card numbers should not be stored");
        }

        // Check CVV storage (should never be stored)
        if (!string.IsNullOrWhiteSpace(cvv))
        {
            violations.Add("CVV should never be stored");
        }

        // Check expiration date (should not store if expired)
        if (expiryDate < DateTime.UtcNow)
        {
            violations.Add("Expired cards should not be processed");
        }

        if (violations.Count > 0)
        {
            _logger.LogWarning("PCI DSS Violations: {Violations}", string.Join(", ", violations));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Generate secure transaction token
    /// </summary>
    public string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var tokenBytes = new byte[32];
        rng.GetBytes(tokenBytes);
        return Convert.ToBase64String(tokenBytes);
    }

    /// <summary>
    /// Hash sensitive data for comparison
    /// </summary>
    public string HashData(string data)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Validate that payment data transmission is secure
    /// </summary>
    public bool ValidateSecureTransmission(bool isHttps, string userAgent, string ipAddress)
    {
        var violations = new List<string>();

        if (!isHttps)
        {
            violations.Add("Payment data transmitted over insecure connection");
        }

        // Log suspicious user agents
        if (userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("crawler", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Suspicious user agent detected: {UserAgent} from IP: {IPAddress}", 
                userAgent, ipAddress);
        }

        if (violations.Count > 0)
        {
            _logger.LogError("Security violations in payment transmission: {Violations}", 
                string.Join(", ", violations));
            return false;
        }

        return true;
    }
}

/// <summary>
/// Exception for security-related issues
/// </summary>
public class SecurityException : Exception
{
    public SecurityException(string message) : base(message) { }
    public SecurityException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Interface for payment security services
/// </summary>
public interface IPaymentSecurityService
{
    /// <summary>
    /// Encrypt sensitive payment data
    /// </summary>
    string EncryptPaymentData(string plainText);

    /// <summary>
    /// Decrypt sensitive payment data
    /// </summary>
    string DecryptPaymentData(string encryptedText);

    /// <summary>
    /// Mask credit card number for display
    /// </summary>
    string MaskCardNumber(string cardNumber);

    /// <summary>
    /// Validate PCI DSS compliant card data handling
    /// </summary>
    bool ValidatePCIDataHandling(string cardNumber, string cvv, DateTime expiryDate);

    /// <summary>
    /// Generate secure transaction token
    /// </summary>
    string GenerateSecureToken();

    /// <summary>
    /// Hash sensitive data for comparison
    /// </summary>
    string HashData(string data);

    /// <summary>
    /// Validate that payment data transmission is secure
    /// </summary>
    bool ValidateSecureTransmission(bool isHttps, string userAgent, string ipAddress);
}