using System.Text.Json;
using WorkFlowDesk.Common.Configuration;

namespace WorkFlowDesk.Common.Services;

/// <summary>Preferencias de usuario persistidas en disco (avatar, etc.).</summary>
public static class UserPreferencesService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static UserPreferencesStore? _store;

    public static event EventHandler<int>? AvatarChanged;

    /// <summary>Índice de avatar guardado para el usuario (0–11).</summary>
    public static int GetAvatarIndex(int userId)
    {
        EnsureLoaded();
        return _store!.AvatarIndices.TryGetValue(userId, out var index)
            ? Math.Clamp(index, 0, AvatarCatalog.Count - 1)
            : userId % AvatarCatalog.Count;
    }

    /// <summary>Guarda el avatar elegido y notifica el cambio.</summary>
    public static void SetAvatarIndex(int userId, int index)
    {
        EnsureLoaded();
        _store!.AvatarIndices[userId] = Math.Clamp(index, 0, AvatarCatalog.Count - 1);
        Save();
        AvatarChanged?.Invoke(null, userId);
    }

    private static void EnsureLoaded()
    {
        if (_store != null)
            return;

        var path = GetPreferencesPath();
        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                _store = JsonSerializer.Deserialize<UserPreferencesStore>(json) ?? new UserPreferencesStore();
            }
            catch
            {
                _store = new UserPreferencesStore();
            }
        }
        else
        {
            _store = new UserPreferencesStore();
        }
    }

    private static void Save()
    {
        var path = GetPreferencesPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(_store, JsonOptions));
    }

    private static string GetPreferencesPath() =>
        Path.Combine(DatabasePaths.GetDataDirectory(), "user-preferences.json");

    private sealed class UserPreferencesStore
    {
        public Dictionary<int, int> AvatarIndices { get; set; } = new();
    }
}
