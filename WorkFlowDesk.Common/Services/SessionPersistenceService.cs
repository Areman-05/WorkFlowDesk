using System.Text.Json;
using WorkFlowDesk.Common.Configuration;

namespace WorkFlowDesk.Common.Services;

/// <summary>Persiste la sesión para restaurarla al reiniciar, con caducidad por inactividad.</summary>
public static class SessionPersistenceService
{
    /// <summary>Días sin abrir la app tras los cuales hay que volver a iniciar sesión.</summary>
    public static readonly TimeSpan MaxIdleTime = TimeSpan.FromDays(2);

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    internal static string? StorageDirectoryOverride { get; set; }

    private static string FilePath =>
        Path.Combine(StorageDirectoryOverride ?? DatabasePaths.GetDataDirectory(), "session.json");

    public static void Save(int userId)
    {
        Write(new PersistedSession
        {
            UserId = userId,
            LastActiveUtc = DateTime.UtcNow
        });
    }

    public static void Touch(int userId)
    {
        if (!File.Exists(FilePath))
        {
            Save(userId);
            return;
        }

        try
        {
            var payload = ReadPayload();
            if (payload == null || payload.UserId != userId)
            {
                Save(userId);
                return;
            }

            payload.LastActiveUtc = DateTime.UtcNow;
            Write(payload);
        }
        catch
        {
            Save(userId);
        }
    }

    /// <summary>Devuelve el usuario guardado si la sesión no ha caducado por inactividad.</summary>
    public static bool TryGetValidSession(out int userId)
    {
        userId = 0;
        var payload = ReadPayload();
        if (payload == null)
            return false;

        if (DateTime.UtcNow - payload.LastActiveUtc > MaxIdleTime)
        {
            Clear();
            return false;
        }

        userId = payload.UserId;
        return true;
    }

    public static void Clear()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    private static PersistedSession? ReadPayload()
    {
        if (!File.Exists(FilePath))
            return null;

        try
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<PersistedSession>(json);
        }
        catch
        {
            return null;
        }
    }

    private static void Write(PersistedSession payload) =>
        File.WriteAllText(FilePath, JsonSerializer.Serialize(payload, JsonOptions));

    private sealed class PersistedSession
    {
        public int UserId { get; set; }
        public DateTime LastActiveUtc { get; set; }

        // Compatibilidad con session.json anterior (SavedAtUtc).
        public DateTime SavedAtUtc
        {
            set
            {
                if (LastActiveUtc == default)
                    LastActiveUtc = value;
            }
        }
    }
}
