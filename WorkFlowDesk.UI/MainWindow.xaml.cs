using System.Windows;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _navigationService = new NavigationService();
            _navigationService.Initialize(ContentArea);
            
            _mainViewModel = new MainViewModel();
            _mainViewModel.NavigateRequested += OnNavigateRequested;
            _mainViewModel.LogoutRequested += OnLogoutRequested;
            DataContext = _mainViewModel;
            
            _navigationService.NavigateTo(NavigationViewFactory.CreateView("Dashboard"));
        }

        private void OnLogoutRequested(object? sender, EventArgs e)
        {
            if (NotificationService.ShowConfirmation(
                    "¿Desea cerrar la sesión actual?",
                    "Cerrar sesión") != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            AuthFlowService.LogoutAndShowLogin(this);
        }

        private void OnNavigateRequested(object? sender, string viewName)
        {
            if (!CanNavigateTo(viewName))
            {
                NotificationService.ShowWarning("No tiene permisos para acceder a esta sección.");
                return;
            }

            _navigationService.NavigateTo(NavigationViewFactory.CreateView(viewName));
        }

        private static bool CanNavigateTo(string viewName) => viewName switch
        {
            "Dashboard" => RolePermissions.CanAccessDashboard,
            "Empleados" => RolePermissions.CanAccessEmpleados,
            "Proyectos" => RolePermissions.CanAccessProyectos,
            "Tareas" => RolePermissions.CanAccessTareas,
            "Clientes" => RolePermissions.CanAccessClientes,
            "Reportes" => RolePermissions.CanAccessReportes,
            "Configuracion" => RolePermissions.CanAccessConfiguracion,
            _ => false
        };
    }
}
