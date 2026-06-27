using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using WorkFlowDesk.Common.Services;

namespace WorkFlowDesk.UI.Helpers;

/// <summary>Carga y cachea imágenes de avatar desde URL (DiceBear).</summary>
public static class AvatarImageLoader
{
    private static readonly HttpClient HttpClient = new();
    private static readonly ConcurrentDictionary<string, Task<BitmapImage?>> Cache = new();
    private static readonly SemaphoreSlim Gate = new(4, 4);

    static AvatarImageLoader()
    {
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WorkFlowDesk/1.0");
        HttpClient.Timeout = TimeSpan.FromSeconds(20);
    }

    public static Task<BitmapImage?> LoadAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.FromResult<BitmapImage?>(null);

        return Cache.GetOrAdd(url, static u => LoadCoreAsync(u));
    }

    public static Task PreloadCatalogAsync() =>
        Task.WhenAll(AvatarCatalog.GetAllUrls().Select(LoadAsync));

    public static bool TryGetCached(string url, out BitmapImage? image)
    {
        if (Cache.TryGetValue(url, out var task)
            && task.IsCompletedSuccessfully
            && task.GetAwaiter().GetResult() is { } cached)
        {
            image = cached;
            return true;
        }

        image = null;
        return false;
    }

    private static async Task<BitmapImage?> LoadCoreAsync(string url)
    {
        await Gate.WaitAsync();
        try
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    var bytes = await HttpClient.GetByteArrayAsync(url);
                    await using var input = new MemoryStream(bytes);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = input;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
                catch when (attempt == 0)
                {
                    await Task.Delay(250);
                }
            }

            return null;
        }
        finally
        {
            Gate.Release();
        }
    }
}
