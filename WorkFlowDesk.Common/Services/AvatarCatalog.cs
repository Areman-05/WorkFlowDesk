namespace WorkFlowDesk.Common.Services;

/// <summary>Catálogo de avatares generados con DiceBear (12 opciones).</summary>
public static class AvatarCatalog
{
    public const int Count = 12;

    /// <summary>URL del avatar para el índice indicado (0–11).</summary>
    public static string GetUrl(int index)
    {
        var safeIndex = Math.Clamp(index, 0, Count - 1);
        return $"https://api.dicebear.com/7.x/adventurer/png?seed=workflowdesk-{safeIndex + 1}&size=128";
    }

    /// <summary>Devuelve las 12 URLs del catálogo.</summary>
    public static IReadOnlyList<string> GetAllUrls()
    {
        return Enumerable.Range(0, Count).Select(GetUrl).ToList();
    }
}
