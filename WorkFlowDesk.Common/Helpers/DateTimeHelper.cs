namespace WorkFlowDesk.Common.Helpers;

/// <summary>Utilidades para formatear y parsear fechas.</summary>
public static class DateTimeHelper
{
    public static string ToDisplayString(DateTime? date, string format = "dd/MM/yyyy")
    {
        return date?.ToString(format) ?? string.Empty;
    }

    public static string ToDisplayString(DateTime date, string format = "dd/MM/yyyy")
    {
        return date.ToString(format);
    }

    public static string ToLongDisplayString(DateTime? date)
    {
        return date?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
    }

    public static bool IsValidDate(DateTime? date)
    {
        return date.HasValue && date.Value != DateTime.MinValue && date.Value != DateTime.MaxValue;
    }

    public static DateTime? ParseNullableDate(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateTime.TryParse(dateString, out var date))
            return date;

        return null;
    }

    public static int DaysBetween(DateTime startDate, DateTime endDate)
    {
        return (endDate - startDate).Days;
    }

    public static int DaysBetween(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
            return 0;

        return DaysBetween(startDate.Value, endDate.Value);
    }

    public static bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
    {
        return date >= startDate && date <= endDate;
    }

    public static DateTime? GetStartOfDay(DateTime? date)
    {
        if (!date.HasValue)
            return null;

        return date.Value.Date;
    }

    public static DateTime? GetEndOfDay(DateTime? date)
    {
        if (!date.HasValue)
            return null;

        return date.Value.Date.AddDays(1).AddTicks(-1);
    }
}
