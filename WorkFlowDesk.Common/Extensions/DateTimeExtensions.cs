namespace WorkFlowDesk.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    public static DateTime StartOfWeek(this DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    public static DateTime EndOfWeek(this DateTime date)
    {
        return date.StartOfWeek().AddDays(6).EndOfDay();
    }

    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    public static DateTime EndOfMonth(this DateTime date)
    {
        return date.StartOfMonth().AddMonths(1).AddDays(-1).EndOfDay();
    }

    public static DateTime StartOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 1, 1);
    }

    public static DateTime EndOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 12, 31).EndOfDay();
    }

    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    public static bool IsWeekday(this DateTime date)
    {
        return !date.IsWeekend();
    }

    public static int DaysUntil(this DateTime date)
    {
        return (date.Date - DateTime.Today).Days;
    }

    public static int DaysSince(this DateTime date)
    {
        return (DateTime.Today - date.Date).Days;
    }

    public static string ToRelativeTime(this DateTime date)
    {
        var timeSpan = DateTime.Now - date;

        if (timeSpan.TotalSeconds < 60)
            return "hace unos segundos";

        if (timeSpan.TotalMinutes < 60)
            return $"hace {(int)timeSpan.TotalMinutes} minuto(s)";

        if (timeSpan.TotalHours < 24)
            return $"hace {(int)timeSpan.TotalHours} hora(s)";

        if (timeSpan.TotalDays < 30)
            return $"hace {(int)timeSpan.TotalDays} día(s)";

        if (timeSpan.TotalDays < 365)
            return $"hace {(int)(timeSpan.TotalDays / 30)} mes(es)";

        return $"hace {(int)(timeSpan.TotalDays / 365)} año(s)";
    }
}
