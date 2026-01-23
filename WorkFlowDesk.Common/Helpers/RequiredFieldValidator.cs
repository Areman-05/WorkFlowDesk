using System.ComponentModel.DataAnnotations;

namespace WorkFlowDesk.Common.Helpers;

public static class RequiredFieldValidator
{
    public static ValidationResult? ValidateRequired(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationResult($"{fieldName} es requerido");
        }
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateRequired<T>(T? value, string fieldName) where T : struct
    {
        if (!value.HasValue)
        {
            return new ValidationResult($"{fieldName} es requerido");
        }
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateRequired(object? value, string fieldName)
    {
        if (value == null)
        {
            return new ValidationResult($"{fieldName} es requerido");
        }
        return ValidationResult.Success;
    }

    public static bool IsValidRequired(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsValidRequired<T>(T? value) where T : struct
    {
        return value.HasValue;
    }

    public static bool IsValidRequired(object? value)
    {
        return value != null;
    }
}
