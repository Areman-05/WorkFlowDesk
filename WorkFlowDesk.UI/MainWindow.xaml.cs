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
            
            NavegarADashboard();
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
                    NavegarADashboard();
                    break;
                case "Empleados":
                    var empleadosService = ServiceLocator.GetService<IEmpleadoService>();
                    var exportService = ServiceLocator.GetService<IExportService>();
                    var empleadosViewModel = new EmpleadosViewModel(empleadosService, exportService);
                    _navigationService.NavigateTo(new EmpleadosView(empleadosViewModel));
                    break;
                case "Proyectos":
                    var proyectosService = ServiceLocator.GetService<IProyectoService>();
                    var exportProyectos = ServiceLocator.GetService<IExportService>();
                    var proyectosViewModel = new ProyectosViewModel(proyectosService, exportProyectos);
                    _navigationService.NavigateTo(new ProyectosView(proyectosViewModel));
                    break;
                case "Tareas":
                    var tareasService = ServiceLocator.GetService<ITareaService>();
                    var exportTareas = ServiceLocator.GetService<IExportService>();
                    var tareasViewModel = new TareasViewModel(tareasService, exportTareas);
                    _navigationService.NavigateTo(new TareasView(tareasViewModel));
                    break;
                case "Clientes":
                    var clientesService = ServiceLocator.GetService<IClienteService>();
                    var exportClientes = ServiceLocator.GetService<IExportService>();
                    var clientesViewModel = new ClientesViewModel(clientesService, exportClientes);
                    _navigationService.NavigateTo(new ClientesView(clientesViewModel));
                    break;
                case "Reportes":
                    var reporteService = ServiceLocator.GetService<IReporteService>();
                    var exportReportes = ServiceLocator.GetService<IExportService>();
                    var reportesViewModel = new ReportesViewModel(reporteService, exportReportes);
                    _navigationService.NavigateTo(new ReportesView(reportesViewModel));
                    break;
                case "Configuracion":
                    var backupSvc = ServiceLocator.GetService<IBackupService>();
                    var dbInitSvc = ServiceLocator.GetService<IDatabaseInitializationService>();
                    var authSvc = ServiceLocator.GetService<IAuthenticationService>();
                    var configVm = new ConfiguracionViewModel(backupSvc, dbInitSvc, authSvc);
                    _navigationService.NavigateTo(new ConfiguracionView(configVm));
                    break;
            }
        }

        private void NavegarADashboard()
        {
            var empleadoService = ServiceLocator.GetService<IEmpleadoService>();
            var proyectoService = ServiceLocator.GetService<IProyectoService>();
            var tareaService = ServiceLocator.GetService<ITareaService>();
            var clienteService = ServiceLocator.GetService<IClienteService>();
            var dashboardViewModel = new DashboardViewModel(empleadoService, proyectoService, tareaService, clienteService);
            _navigationService.NavigateTo(new DashboardView(dashboardViewModel));
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