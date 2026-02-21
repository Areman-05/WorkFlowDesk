namespace WorkFlowDesk.Common.Helpers;

/// <summary>Utilidades para fechas (formato, rangos, días entre).</summary>
public static class DateHelper
{
    /// <summary>Formatea la fecha como dd/MM/yyyy o cadena vacía si es null.</summary>
    public static string ToShortDateString(DateTime? date)
    {
        return date?.ToString("dd/MM/yyyy") ?? string.Empty;
    }

    /// <summary>Formatea la fecha con hora como dd/MM/yyyy HH:mm.</summary>
    public static string ToLongDateString(DateTime? date)
    {
        return date?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
    }

    public static bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
    {
        return date >= startDate && date <= endDate;
    }

    public static int DaysBetween(DateTime startDate, DateTime endDate)
    {
        return (endDate - startDate).Days;
    }
}
