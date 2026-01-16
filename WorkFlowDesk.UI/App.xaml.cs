using System.Windows;
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
            base.OnStartup(e);
            ServiceLocator.ConfigureServices();
        }
    }
}
