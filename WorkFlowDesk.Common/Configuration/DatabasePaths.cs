namespace WorkFlowDesk.Common.Configuration;

/// <summary>Rutas de la base de datos SQLite embebida (AppData local).</summary>
public static class DatabasePaths
{
    public const string DatabaseFileName = "workflowdesk.db";

    /// <summary>Directorio de datos de la aplicación en %LocalAppData%/WorkFlowDesk.</summary>
    public static string GetDataDirectory()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WorkFlowDesk");

        Directory.CreateDirectory(directory);
        return directory;
    }

    /// <summary>Ruta completa del archivo SQLite.</summary>
    public static string GetDatabaseFilePath() =>
        Path.Combine(GetDataDirectory(), DatabaseFileName);

    /// <summary>Cadena de conexión EF Core para SQLite.</summary>
    public static string GetConnectionString() =>
        $"Data Source={GetDatabaseFilePath()}";
}
