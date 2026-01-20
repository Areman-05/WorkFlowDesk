using System.Windows;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
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
            base.OnStartup(e);
            ServiceLocator.ConfigureServices();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var authService = ServiceLocator.GetService<Services.Interfaces.IAuthenticationService>();
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
