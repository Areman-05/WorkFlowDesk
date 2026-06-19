namespace WorkFlowDesk.Common.Services;

/// <summary>Momento de arranque de la aplicación para métricas de tiempo activo.</summary>
public static class AppRuntimeInfo
{
    public static DateTime StartedAt { get; private set; } = DateTime.Now;

    public static void MarkStarted() => StartedAt = DateTime.Now;

    public static string GetUptimeFormatted()
    {
        var span = DateTime.Now - StartedAt;
        return $"{span.Days}d {span.Hours:00}h {span.Minutes:00}m";
    }
}
