using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;

namespace WorkFlowDesk.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            DispatcherUnhandledException += (_, args) =>
            {
                MessageBox.Show(
                    $"Error inesperado:\n\n{args.Exception.Message}",
                    "WorkFlowDesk",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };

            AppConfig.LoadFromFile();
            AppRuntimeInfo.MarkStarted();
            DatabasePaths.GetDataDirectory();
            ServiceLocator.ConfigureServices();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.Dispose();
            base.OnExit(e);
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!await InitializeDatabaseAsync())
            {
                Shutdown();
                return;
            }

            _ = AvatarImageLoader.PreloadCatalogAsync();

            AuthFlowService.ShowLoginFlow();
        }

        private async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                var dbInitService = ServiceLocator.Provider.GetRequiredService<IDatabaseInitializationService>();
                await dbInitService.InitializeAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo inicializar la base de datos.\n\n{ex.Message}",
                    "WorkFlowDesk",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
        }
    }
}
