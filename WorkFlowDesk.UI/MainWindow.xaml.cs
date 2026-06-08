using System.Windows;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.UI.Views;
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
            
            // Navegar a Dashboard por defecto
            _navigationService.NavigateTo<DashboardView>();
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

            switch (viewName)
            {
                case "Dashboard":
                    var empleadoService = ServiceLocator.GetService<IEmpleadoService>();
                    var proyectoService = ServiceLocator.GetService<IProyectoService>();
                    var tareaService = ServiceLocator.GetService<ITareaService>();
                    var clienteService = ServiceLocator.GetService<IClienteService>();
                    var dashboardViewModel = new DashboardViewModel(empleadoService, proyectoService, tareaService, clienteService);
                    var dashboardView = new DashboardView();
                    dashboardView.DataContext = dashboardViewModel;
                    _navigationService.NavigateTo(dashboardView);
                    break;
                case "Empleados":
                    var empleadosService = ServiceLocator.GetService<IEmpleadoService>();
                    var empleadosViewModel = new EmpleadosViewModel(empleadosService);
                    _navigationService.NavigateTo(new EmpleadosView(empleadosViewModel));
                    break;
                case "Proyectos":
                    var proyectosService = ServiceLocator.GetService<IProyectoService>();
                    var proyectosViewModel = new ProyectosViewModel(proyectosService);
                    _navigationService.NavigateTo(new ProyectosView(proyectosViewModel));
                    break;
                case "Tareas":
                    var tareasService = ServiceLocator.GetService<ITareaService>();
                    var tareasViewModel = new TareasViewModel(tareasService);
                    _navigationService.NavigateTo(new TareasView(tareasViewModel));
                    break;
                case "Clientes":
                    var clientesService = ServiceLocator.GetService<IClienteService>();
                    var clientesViewModel = new ClientesViewModel(clientesService);
                    _navigationService.NavigateTo(new ClientesView(clientesViewModel));
                    break;
                case "Reportes":
                    var reporteService = ServiceLocator.GetService<IReporteService>();
                    var reportesViewModel = new ReportesViewModel(reporteService);
                    _navigationService.NavigateTo(new ReportesView(reportesViewModel));
                    break;
                case "Configuracion":
                    var backupSvc = ServiceLocator.GetService<IBackupService>();
                    var dbInitSvc = ServiceLocator.GetService<IDatabaseInitializationService>();
                    var configVm = new ConfiguracionViewModel(backupSvc, dbInitSvc);
                    _navigationService.NavigateTo(new ConfiguracionView(configVm));
                    break;
            }
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