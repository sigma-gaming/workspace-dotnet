using System.ComponentModel.DataAnnotations;

namespace Common.Validation.Attributes;

public class EachInRangeAttribute(int min, int max)
    : ValidationAttribute($"Each number must be between {min} and {max}")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable<int> numbers)
            return ValidationResult.Success;

        foreach (var number in numbers)
            if (number < min || number > max)
            {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

        return ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name, min, max);
    }
}