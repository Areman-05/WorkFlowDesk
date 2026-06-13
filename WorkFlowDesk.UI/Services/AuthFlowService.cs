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
    /// <summary>Muestra el diálogo de login y abre la ventana principal si las credenciales son válidas.</summary>
    public static void ShowLoginFlow()
    {
        var authService = ServiceLocator.Provider.GetRequiredService<IAuthenticationService>();
        var loginViewModel = new LoginViewModel(authService);
        var loginView = new LoginView(loginViewModel);

        loginViewModel.LoginExitoso += (_, usuario) => OpenMainApplication(loginView, usuario);

        loginViewModel.AbrirRegistroRequested += (_, _) =>
        {
            var usuario = ShowRegisterDialog();
            if (usuario != null)
            {
                OpenMainApplication(loginView, usuario);
            }
        };

        loginView.ShowDialog();

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

    /// <summary>Muestra el formulario de registro y devuelve el usuario creado si tuvo éxito.</summary>
    public static Usuario? ShowRegisterDialog()
    {
        var usuarioService = ServiceLocator.Provider.GetRequiredService<IUsuarioService>();
        var authService = ServiceLocator.Provider.GetRequiredService<IAuthenticationService>();
        var registerViewModel = new RegisterViewModel(usuarioService, authService);
        var registerView = new RegisterView(registerViewModel);

        Usuario? usuarioRegistrado = null;
        registerViewModel.RegistroExitoso += (_, usuario) => usuarioRegistrado = usuario;

        registerView.ShowDialog();
        return usuarioRegistrado;
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
