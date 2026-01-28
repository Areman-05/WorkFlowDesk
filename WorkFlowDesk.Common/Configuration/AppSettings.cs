namespace WorkFlowDesk.Common.Configuration;

/// <summary>Configuración general de la aplicación.</summary>
public class AppSettings
{
    public string ApplicationName { get; set; } = "WorkFlowDesk";
    public string Version { get; set; } = "1.0.0";
    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 100;
    public bool EnableLogging { get; set; } = true;
    public string LogLevel { get; set; } = "Info";
    public int CacheExpirationMinutes { get; set; } = 30;
    public bool EnableNotifications { get; set; } = true;
}

public static class AppConfig
{
    private static AppSettings? _settings;

    public static AppSettings Settings
    {
        get
        {
            if (_settings == null)
            {
                _settings = new AppSettings();
            }
            return _settings;
        }
        set => _settings = value;
    }

    public static void LoadFromFile(string filePath)
    {
        // Por ahora retornamos configuración por defecto
        // Se puede mejorar para leer desde archivo JSON
        Settings = new AppSettings();
    }
}
