using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Services;

namespace WorkFlowDesk.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppConfig.LoadFromFile();
            ServiceLocator.ConfigureServices();
            base.OnStartup(e);
            _ = InitializeDatabaseAsync();
        }

        /// <summary>Inicializa la base de datos y el seed en segundo plano.</summary>
        private async Task InitializeDatabaseAsync()
        {
            try
            {
                var dbInitService = ServiceLocator.Provider.GetRequiredService<IDatabaseInitializationService>();
                await dbInitService.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al inicializar base de datos: {ex.Message}");
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AuthFlowService.ShowLoginFlow();
        }
    }
}
