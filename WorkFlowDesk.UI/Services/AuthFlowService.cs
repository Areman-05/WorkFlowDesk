using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Models;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

/// <summary>Gestiona login, restauración de sesión y apertura de la aplicación principal.</summary>
public static class AuthFlowService
{
    /// <summary>Intenta restaurar sesión guardada; si no, muestra el login.</summary>
    public static async Task StartApplicationAsync()
    {
        if (await TryRestoreSavedSessionAsync())
        {
            await OpenMainApplicationAsync(isRestoredSession: true);
            return;
        }

        ShowLoginFlow();
    }

    /// <summary>Muestra la ventana de autenticación y abre la aplicación principal si las credenciales son válidas.</summary>
    public static void ShowLoginFlow()
    {
        var authService = ServiceLocator.Provider.GetRequiredService<IAuthenticationService>();
        var usuarioService = ServiceLocator.Provider.GetRequiredService<IUsuarioService>();
        var shell = new AuthShellViewModel(authService, usuarioService);
        var authWindow = new AuthWindow(shell);

        shell.AutenticacionExitosa += (_, usuario) =>
            _ = OpenMainApplicationAfterAuthAsync(authWindow, usuario);

        authWindow.ShowDialog();

        if (!SessionService.IsAuthenticated)
            Application.Current.Shutdown();
    }

    /// <summary>Cierra la sesión actual y vuelve al login sin cerrar la aplicación.</summary>
    public static void LogoutAndShowLogin(Window? windowToClose = null)
    {
        SessionPersistenceService.Clear();
        SessionService.ClearSession();
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        windowToClose?.Close();
        ShowLoginFlow();
    }

    private static async Task<bool> TryRestoreSavedSessionAsync()
    {
        if (!SessionPersistenceService.TryGetValidSession(out var userId))
            return false;

        try
        {
            var usuarioService = ServiceLocator.Provider.GetRequiredService<IUsuarioService>();
            var usuario = await usuarioService.GetByIdAsync(userId);
            if (usuario == null || !usuario.Activo)
            {
                SessionPersistenceService.Clear();
                return false;
            }

            SessionPersistenceService.Touch(usuario.Id);
            SessionService.SetCurrentUser(usuario);
            ApplyUserPreferences(usuario.Id);
            return true;
        }
        catch
        {
            SessionPersistenceService.Clear();
            return false;
        }
    }

    private static async Task OpenMainApplicationAfterAuthAsync(Window dialogToClose, Usuario usuario)
    {
        try
        {
            SessionService.SetCurrentUser(usuario);
            SessionPersistenceService.Save(usuario.Id);
            ApplyUserPreferences(usuario.Id);

            var splash = SplashScreenService.ShowImmediately(SplashScreenKind.PostLogin);
            dialogToClose.Close();

            await SplashScreenService.RunPostLoginSplashAsync(splash);
            ShowMainWindow();
        }
        catch (Exception ex)
        {
            SessionPersistenceService.Clear();
            SessionService.ClearSession();
            MessageBox.Show(
                $"No se pudo abrir la aplicación.\n\n{ex.Message}",
                "WorkFlowDesk",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static async Task OpenMainApplicationAsync(bool isRestoredSession)
    {
        if (isRestoredSession)
            await SplashScreenService.RunQuickSplashAsync();
        else
            await SplashScreenService.RunPostLoginSplashAsync();

        ShowMainWindow();
    }

    private static void ShowMainWindow()
    {
        var mainWindow = new MainWindow();
        Application.Current.MainWindow = mainWindow;
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        mainWindow.Show();
    }

    private static void ApplyUserPreferences(int userId)
    {
        AppThemeService.ApplyForUser(userId);
        var prefs = UserPreferencesService.GetProfileData(userId);
        LocalizationService.Apply(prefs.Idioma);
        DesktopNotificationService.SetEnabled(prefs.NotificacionesEscritorio);
    }
}
