using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Services;

namespace WorkFlowDesk.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppConfig.LoadFromFile();
            DatabasePaths.GetDataDirectory();
            ServiceLocator.ConfigureServices();
            base.OnStartup(e);
            _ = InitializeDatabaseAsync();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.Dispose();
            base.OnExit(e);
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                var dbInitService = ServiceLocator.Provider.GetRequiredService<IDatabaseInitializationService>();
                await dbInitService.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo inicializar la base de datos.\n\n{ex.Message}",
                    "WorkFlowDesk",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AuthFlowService.ShowLoginFlow();
        }
    }
}
