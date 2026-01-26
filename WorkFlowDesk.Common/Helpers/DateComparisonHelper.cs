namespace WorkFlowDesk.Common.Helpers;

public static class DateComparisonHelper
{
    public static bool IsToday(DateTime date)
    {
        return date.Date == DateTime.Today;
    }

    public static bool IsToday(DateTime? date)
    {
        return date.HasValue && IsToday(date.Value);
    }

    public static bool IsThisWeek(DateTime date)
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        return date >= startOfWeek && date < endOfWeek;
    }

    public static bool IsThisMonth(DateTime date)
    {
        return date.Year == DateTime.Now.Year && date.Month == DateTime.Now.Month;
    }

    public static bool IsThisYear(DateTime date)
    {
        return date.Year == DateTime.Now.Year;
    }

    public static bool IsPast(DateTime date)
    {
        return date < DateTime.Now;
    }

    public static bool IsFuture(DateTime date)
    {
        return date > DateTime.Now;
    }

    public static bool IsPast(DateTime? date)
    {
        return date.HasValue && IsPast(date.Value);
    }

    public static bool IsFuture(DateTime? date)
    {
        return date.HasValue && IsFuture(date.Value);
    }

    public static int DaysUntil(DateTime date)
    {
        return (date.Date - DateTime.Today).Days;
    }

    public static int DaysSince(DateTime date)
    {
        return (DateTime.Today - date.Date).Days;
    }

    public static bool IsOverdue(DateTime? dueDate)
    {
        return dueDate.HasValue && IsPast(dueDate.Value);
    }

    public static bool IsDueSoon(DateTime? dueDate, int daysThreshold = 7)
    {
        if (!dueDate.HasValue)
            return false;

        var daysUntil = DaysUntil(dueDate.Value);
        return daysUntil >= 0 && daysUntil <= daysThreshold;
    }
}
