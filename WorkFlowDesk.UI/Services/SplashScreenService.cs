using System.Windows;
using WorkFlowDesk.UI.Models;
using WorkFlowDesk.UI.Views;

namespace WorkFlowDesk.UI.Services;

/// <summary>Muestra pantallas de carga y ejecuta la precarga asociada.</summary>
public static class SplashScreenService
{
    public static async Task RunPostLoginSplashAsync(SplashScreenWindow? existing = null)
    {
        var splash = existing ?? ShowImmediately(SplashScreenKind.PostLogin);
        await CompleteAsync(splash, isPostLogin: true);
    }

    public static async Task RunQuickSplashAsync(SplashScreenWindow? existing = null)
    {
        var splash = existing ?? ShowImmediately(SplashScreenKind.Quick);
        await CompleteAsync(splash, isPostLogin: false);
    }

    /// <summary>Muestra la splash al instante para tapar la ventana de login u otras.</summary>
    public static SplashScreenWindow ShowImmediately(SplashScreenKind kind)
    {
        var splash = new SplashScreenWindow(kind);
        splash.Show();
        splash.UpdateLayout();
        splash.Activate();
        Application.Current.MainWindow = splash;
        return splash;
    }

    private static async Task CompleteAsync(SplashScreenWindow splash, bool isPostLogin)
    {
        try
        {
            await splash.PlayIntroAsync();

            var bootstrap = isPostLogin
                ? WorkspaceBootstrapService.BootstrapPostLoginAsync(CancellationToken.None)
                : WorkspaceBootstrapService.BootstrapQuickRestoreAsync(CancellationToken.None);

            var loaderSequence = isPostLogin
                ? splash.RunPostLoginBarSequenceAsync()
                : splash.RunQuickLoaderSequenceAsync();

            await Task.WhenAll(bootstrap, loaderSequence);
            await splash.FadeOutAsync();
        }
        finally
        {
            splash.Close();
        }
    }
}
