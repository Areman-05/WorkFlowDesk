using System.Text.Json;

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

/// <summary>Punto de acceso global a la configuración de la aplicación.</summary>
public static class AppConfig
{
    private static AppSettings? _settings;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string ConfigFilePath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json");

    public static AppSettings Settings
    {
        get
        {
            _settings ??= new AppSettings();
            return _settings;
        }
        set => _settings = value;
    }

    /// <summary>Carga la configuración desde appconfig.json o usa valores por defecto.</summary>
    public static void LoadFromFile(string? filePath = null)
    {
        var path = filePath ?? ConfigFilePath;
        if (!File.Exists(path))
        {
            Settings = new AppSettings();
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            Settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }
        catch
        {
            Settings = new AppSettings();
        }
    }

    /// <summary>Guarda la configuración actual en appconfig.json.</summary>
    public static void SaveToFile(string? filePath = null)
    {
        var path = filePath ?? ConfigFilePath;
        var json = JsonSerializer.Serialize(Settings, JsonOptions);
        File.WriteAllText(path, json);
    }
}
