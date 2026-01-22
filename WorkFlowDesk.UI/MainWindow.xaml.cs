using System.Windows;
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
            DataContext = _mainViewModel;
            
            // Navegar a Dashboard por defecto
            _navigationService.NavigateTo<DashboardView>();
        }

        private void OnNavigateRequested(object? sender, string viewName)
        {
            switch (viewName)
            {
                case "Dashboard":
                    var empleadoService = ServiceLocator.GetService<Services.Interfaces.IEmpleadoService>();
                    var proyectoService = ServiceLocator.GetService<Services.Interfaces.IProyectoService>();
                    var tareaService = ServiceLocator.GetService<Services.Interfaces.ITareaService>();
                    var clienteService = ServiceLocator.GetService<Services.Interfaces.IClienteService>();
                    var dashboardViewModel = new DashboardViewModel(empleadoService, proyectoService, tareaService, clienteService);
                    var dashboardView = new DashboardView();
                    dashboardView.DataContext = dashboardViewModel;
                    _navigationService.NavigateTo(dashboardView);
                    break;
                case "Empleados":
                    var empleadosService = ServiceLocator.GetService<Services.Interfaces.IEmpleadoService>();
                    var empleadosViewModel = new EmpleadosViewModel(empleadosService);
                    _navigationService.NavigateTo(new EmpleadosView(empleadosViewModel));
                    break;
                case "Proyectos":
                    var proyectosService = ServiceLocator.GetService<Services.Interfaces.IProyectoService>();
                    var proyectosViewModel = new ProyectosViewModel(proyectosService);
                    _navigationService.NavigateTo(new ProyectosView(proyectosViewModel));
                    break;
                case "Tareas":
                    var tareasService = ServiceLocator.GetService<Services.Interfaces.ITareaService>();
                    var tareasViewModel = new TareasViewModel(tareasService);
                    _navigationService.NavigateTo(new TareasView(tareasViewModel));
                    break;
                case "Clientes":
                    var clientesService = ServiceLocator.GetService<Services.Interfaces.IClienteService>();
                    var clientesViewModel = new ClientesViewModel(clientesService);
                    _navigationService.NavigateTo(new ClientesView(clientesViewModel));
                    break;
                case "Reportes":
                    var reporteService = ServiceLocator.GetService<Services.Interfaces.IReporteService>();
                    var reportesViewModel = new ReportesViewModel(reporteService);
                    _navigationService.NavigateTo(new ReportesView(reportesViewModel));
                    break;
            }
        }
    }
}