namespace WorkFlowDesk.ViewModel.Helpers;

/// <summary>Iconografía y color por sector de cliente (empresa, no persona).</summary>
public static class ClienteSectorHelper
{
    public sealed record SectorVisual(
        string Sector,
        string IconGlyph,
        string Fondo,
        string IconColor);

    public static SectorVisual Resolver(string empresa, string nombre)
    {
        var sector = InferirSector(empresa, nombre);
        return ObtenerVisual(sector);
    }

    public static string InferirSector(string empresa, string nombre)
    {
        var text = $"{empresa} {nombre}".ToLowerInvariant();

        if (Contiene(text, "bbva", "banco", "finanz", "bank", "capital", "seguros", "invers"))
            return "Banca";

        if (Contiene(text, "nike", "retail", "shop", "ecommerce", "tienda", "comercio", "moda"))
            return "Ecommerce";

        if (Contiene(text, "tech", "dev", "software", "digital", "global dev", "techcorp", "data", "cloud", "app "))
            return "Tecnología";

        if (Contiene(text, "lab", "ciencia", "bio", "pharma", "research", "innovalab", "r+d", "i+d"))
            return "Ciencia";

        if (Contiene(text, "factory", "industr", "manufact", "metal", "automot", "logistic", "energ"))
            return "Industria";

        if (Contiene(text, "media", "studio", "flow", "creativ", "marketing", "audiovisual", "diseño"))
            return "Medios";

        return "Servicios";
    }

    public static SectorVisual ObtenerVisual(string sector) => sector switch
    {
        "Banca" => new SectorVisual("Banca", "\uE825", "#26FD933D", "#944A00"),
        "Tecnología" => new SectorVisual("Tecnología", "\uE770", "#269E00B5", "#9E00B5"),
        "Ecommerce" => new SectorVisual("Ecommerce", "\uE719", "#26A45E86", "#87466D"),
        "Ciencia" => new SectorVisual("Ciencia", "\uE7BF", "#260EA5E9", "#0284C7"),
        "Industria" => new SectorVisual("Industria", "\uE90F", "#26575863", "#374151"),
        "Medios" => new SectorVisual("Medios", "\uE714", "#264338CA", "#4338CA"),
        _ => new SectorVisual("Servicios", "\uE821", "#266B7280", "#524251")
    };

    private static bool Contiene(string text, params string[] terms) =>
        terms.Any(text.Contains);
}
