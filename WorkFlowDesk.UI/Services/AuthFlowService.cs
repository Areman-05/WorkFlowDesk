using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

/// <summary>Gestiona el flujo de login y reapertura de sesión tras cerrar sesión.</summary>
public static class AuthFlowService
{
    /// <summary>Muestra la ventana de autenticación y abre la aplicación principal si las credenciales son válidas.</summary>
    public static void ShowLoginFlow()
    {
        var authService = ServiceLocator.Provider.GetRequiredService<IAuthenticationService>();
        var usuarioService = ServiceLocator.Provider.GetRequiredService<IUsuarioService>();
        var shell = new AuthShellViewModel(authService, usuarioService);
        var authWindow = new AuthWindow(shell);

        shell.AutenticacionExitosa += (_, usuario) => OpenMainApplication(authWindow, usuario);

        authWindow.ShowDialog();

        if (!SessionService.IsAuthenticated)
        {
            Application.Current.Shutdown();
        }
    }

    /// <summary>Cierra la sesión actual y vuelve a mostrar el login.</summary>
    public static void LogoutAndShowLogin(Window? windowToClose = null)
    {
        SessionService.ClearSession();
        windowToClose?.Close();
        ShowLoginFlow();
    }

    private static void OpenMainApplication(Window dialogToClose, Usuario usuario)
    {
        try
        {
            SessionService.SetCurrentUser(usuario);

            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            mainWindow.Show();

            dialogToClose.DialogResult = true;
            dialogToClose.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo abrir la aplicación.\n\n{ex.Message}",
                "WorkFlowDesk",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
