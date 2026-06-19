using System.Globalization;

namespace WorkFlowDesk.Common.Services;

/// <summary>Aplica el idioma de interfaz seleccionado por el usuario.</summary>
public static class LocalizationService
{
    public static void Apply(string idioma)
    {
        var culture = idioma switch
        {
            "English (US)" => new CultureInfo("en-US"),
            "Deutsch" => new CultureInfo("de-DE"),
            _ => new CultureInfo("es-ES")
        };

        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
