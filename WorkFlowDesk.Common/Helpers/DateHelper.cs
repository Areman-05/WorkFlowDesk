namespace WorkFlowDesk.Common.Helpers;

public static class DateHelper
{
    public static string ToShortDateString(DateTime? date)
    {
        return date?.ToString("dd/MM/yyyy") ?? string.Empty;
    }

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
