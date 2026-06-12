using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

/// <summary>Gestiona el flujo de login y reapertura de sesión tras cerrar sesión.</summary>
public static class AuthFlowService
{
    /// <summary>Muestra el diálogo de login y abre la ventana principal si las credenciales son válidas.</summary>
    public static void ShowLoginFlow()
    {
        var authService = ServiceLocator.Provider.GetRequiredService<IAuthenticationService>();
        var loginViewModel = new LoginViewModel(authService);
        var loginView = new LoginView(loginViewModel);

        loginViewModel.LoginExitoso += (_, usuario) =>
        {
            SessionService.SetCurrentUser(usuario);
            loginView.Close();

            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        };

        loginView.ShowDialog();
    }

    /// <summary>Cierra la sesión actual y vuelve a mostrar el login.</summary>
    public static void LogoutAndShowLogin(Window? windowToClose = null)
    {
        SessionService.ClearSession();
        windowToClose?.Close();
        ShowLoginFlow();
    }
}
