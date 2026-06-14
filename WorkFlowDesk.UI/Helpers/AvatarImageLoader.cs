using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;

namespace WorkFlowDesk.UI.Helpers;

/// <summary>Carga imágenes de avatar desde URL (DiceBear).</summary>
public static class AvatarImageLoader
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<BitmapImage?> LoadAsync(string url)
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
        catch
        {
            return null;
        }
    }
}
