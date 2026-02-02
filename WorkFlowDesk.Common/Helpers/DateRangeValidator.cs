using System.ComponentModel.DataAnnotations;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Validador de rangos de fechas y fechas en el pasado/futuro.</summary>
public static class DateRangeValidator
{
    public static ValidationResult? ValidateDateRange(DateTime startDate, DateTime endDate, string fieldName = "Rango de fechas")
    {
        if (endDate < startDate)
        {
            return new ValidationResult($"{fieldName}: La fecha de fin debe ser posterior a la fecha de inicio");
        }
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateDateRange(DateTime? startDate, DateTime? endDate, string fieldName = "Rango de fechas")
    {
        if (!startDate.HasValue || !endDate.HasValue)
            return ValidationResult.Success;

        return ValidateDateRange(startDate.Value, endDate.Value, fieldName);
    }

    public static ValidationResult? ValidateDateNotInPast(DateTime date, string fieldName)
    {
        if (date < DateTime.Today)
        {
            return new ValidationResult($"{fieldName} no puede ser una fecha pasada");
        }
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateDateNotInFuture(DateTime date, string fieldName)
    {
        if (date > DateTime.Today)
        {
            return new ValidationResult($"{fieldName} no puede ser una fecha futura");
        }
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateDateWithinRange(DateTime date, DateTime minDate, DateTime maxDate, string fieldName)
    {
        if (date < minDate || date > maxDate)
        {
            return new ValidationResult($"{fieldName} debe estar entre {minDate:dd/MM/yyyy} y {maxDate:dd/MM/yyyy}");
        }
        return ValidationResult.Success;
    }

    public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return endDate >= startDate;
    }

    public static bool IsValidDateRange(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
            return true;

        return IsValidDateRange(startDate.Value, endDate.Value);
    }
}
