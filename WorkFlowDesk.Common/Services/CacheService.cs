using System.Collections.Concurrent;

namespace WorkFlowDesk.Common.Services;

/// <summary>Servicio de caché en memoria con expiración configurable.</summary>
public class CacheService
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var expirationTime = DateTime.Now.Add(expiration ?? _defaultExpiration);
        _cache[key] = new CacheItem
        {
            Value = value,
            ExpirationTime = expirationTime
        };
    }

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

    public bool TryGet<T>(string key, out T? value)
    {
        value = Get<T>(key);
        return value != null;
    }

    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }

    public void Clear()
    {
        _cache.Clear();
    }

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
