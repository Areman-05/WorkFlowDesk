using System.Collections.Concurrent;

namespace WorkFlowDesk.Common.Services;

/// <summary>Servicio de caché en memoria con expiración configurable.</summary>
public class CacheService
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    /// <summary>Guarda un valor en caché con la clave y opcionalmente tiempo de expiración.</summary>
    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var expirationTime = DateTime.Now.Add(expiration ?? _defaultExpiration);
        _cache[key] = new CacheItem
        {
            Value = value,
            ExpirationTime = expirationTime
        };
    }

    /// <summary>Obtiene un valor por clave; devuelve default si no existe o ha expirado.</summary>
    public T? Get<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var item))
            return default;

        if (DateTime.Now > item.ExpirationTime)
        {
            _cache.TryRemove(key, out _);
            return default;
        }

        return (T?)item.Value;
    }

    /// <summary>Intenta obtener un valor por clave; devuelve true si existe y no ha expirado.</summary>
    public bool TryGet<T>(string key, out T? value)
    {
        value = Get<T>(key);
        return value != null;
    }

    /// <summary>Elimina una entrada de la caché por clave.</summary>
    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }

    /// <summary>Vacía toda la caché.</summary>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <summary>Elimina todas las entradas que ya han expirado.</summary>
    public void CleanExpired()
    {
        var expiredKeys = _cache
            .Where(kvp => DateTime.Now > kvp.Value.ExpirationTime)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _cache.TryRemove(key, out _);
        }
    }

    private class CacheItem
    {
        public object? Value { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
