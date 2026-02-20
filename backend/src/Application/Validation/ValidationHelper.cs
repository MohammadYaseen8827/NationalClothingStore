using System.Text.RegularExpressions;

namespace NationalClothingStore.Application.Validation;

public static class ValidationHelper
{
    public static readonly Regex HtmlTagRegex = new(@"<[^>]*>", RegexOptions.Compiled);
    public static readonly Regex SqlInjectionRegex = new(@"(?i)(union|select|insert|update|delete|drop|create|alter|exec|execute|script)", RegexOptions.Compiled);

    public static string SanitizeHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return HtmlTagRegex.Replace(input ?? string.Empty, string.Empty);
    }

    public static string SanitizeSql(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return SqlInjectionRegex.Replace(input ?? string.Empty, string.Empty);
    }

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return email.Contains('@') && email.Contains('.') && email.Length > 5 && email.Length <= 254;
    }

    public static bool IsValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;
        var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
        return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
    }

    public static string SanitizeAlphaNumeric(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return new string(input.Where(c => char.IsLetterOrDigit(c)).ToArray());
    }

    public static string Truncate(string? input, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return input.Length > maxLength ? input[..maxLength] : input;
    }
}
