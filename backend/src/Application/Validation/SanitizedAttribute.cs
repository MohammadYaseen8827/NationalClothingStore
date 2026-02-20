using System.ComponentModel.DataAnnotations;

namespace NationalClothingStore.Application.Validation;

public class SanitizedAttribute : ValidationAttribute
{
    private readonly bool _allowHtml;
    private readonly int _maxLength;

    public SanitizedAttribute(bool allowHtml = false, int maxLength = 1000)
    {
        _allowHtml = allowHtml;
        _maxLength = maxLength;
    }

}
