using System.Reflection;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Información de versión de la aplicación.</summary>
public static class AppInfo
{
    public static string Version =>
        Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "1.0.0";
}
