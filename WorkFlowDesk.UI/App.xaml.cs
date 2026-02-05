using System.Windows;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Configurar DI antes de base.OnStartup para que Application_Startup tenga servicios disponibles
            ServiceLocator.ConfigureServices();
            base.OnStartup(e);
            // Inicializar BD en segundo plano para no bloquear la ventana de login
            _ = InitializeDatabaseAsync();
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                var dbInitService = ServiceLocator.GetService<IDatabaseInitializationService>();
                await dbInitService.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al inicializar base de datos: {ex.Message}");
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var authService = ServiceLocator.GetService<IAuthenticationService>();
            var loginViewModel = new LoginViewModel(authService);
            var loginView = new LoginView(loginViewModel);

            loginViewModel.LoginExitoso += (s, usuario) =>
            {
                SessionService.SetCurrentUser(usuario);
                loginView.Close();
                
                var mainWindow = new MainWindow();
                mainWindow.Show();
            };

            loginView.ShowDialog();
        }
    }
}
