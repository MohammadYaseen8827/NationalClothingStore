namespace NationalClothingStore.Application.Common;

/// <summary>
/// Result of validation operations
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

    public static ValidationResult Success() => new ValidationResult { IsValid = true };

    public static ValidationResult Failure(params string[] errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    public static ValidationResult Failure(IEnumerable<string> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    public void AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
    }

    public void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            AddError(error);
        }
    }
}
