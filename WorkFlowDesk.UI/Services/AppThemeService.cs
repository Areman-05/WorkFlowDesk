using System.Windows;
using WorkFlowDesk.Common.Services;

namespace WorkFlowDesk.UI.Services;

/// <summary>Aplica temas Claro u Oscuro intercambiando el diccionario de colores global.</summary>
public static class AppThemeService
{
    public const string TemaClaro = "Claro";
    public const string TemaOscuro = "Oscuro";

    private const string ColorsLightUri = "/WorkFlowDesk.UI;component/Styles/StitchThemeColorsLight.xaml";
    private const string ColorsDarkUri = "/WorkFlowDesk.UI;component/Styles/StitchThemeColorsDark.xaml";

    private static ResourceDictionary? _activeColors;
    private static string _modoActual = TemaClaro;

    public static event EventHandler<string>? ThemeChanged;

    public static string ModoActual => _modoActual;

    public static void ApplyForUser(int userId)
    {
        var prefs = UserPreferencesService.GetProfileData(userId);
        var tema = NormalizarModo(prefs.Tema);
        Apply(tema);
    }

    public static void Apply(string modo)
    {
        var normalizado = NormalizarModo(modo);
        if (_modoActual == normalizado && _activeColors != null)
            return;

        _modoActual = normalizado;
        var oscuro = normalizado == TemaOscuro;
        IntercambiarDiccionarioColores(oscuro ? ColorsDarkUri : ColorsLightUri);
        ThemeChanged?.Invoke(null, _modoActual);
    }

    public static string NormalizarModo(string? modo) =>
        modo == TemaOscuro ? TemaOscuro : TemaClaro;

    private static void IntercambiarDiccionarioColores(string uri)
    {
        var merged = Application.Current.Resources.MergedDictionaries;

        for (var i = merged.Count - 1; i >= 0; i--)
        {
            var src = merged[i].Source?.OriginalString ?? string.Empty;
            if (src.Contains("StitchThemeColorsLight", StringComparison.OrdinalIgnoreCase)
                || src.Contains("StitchThemeColorsDark", StringComparison.OrdinalIgnoreCase))
            {
                merged.RemoveAt(i);
            }
        }

        _activeColors = new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,{uri}", UriKind.Absolute)
        };

        merged.Insert(0, _activeColors);
    }
}
