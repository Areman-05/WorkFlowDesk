using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

/// <summary>Crea vistas y ViewModels para la navegación principal.</summary>
public static class NavigationViewFactory
{
    public static UserControl CreateView(string viewName)
    {
        return viewName switch
        {
            "Dashboard" => CreateDashboard(),
            "Empleados" => CreateEmpleados(),
            "Proyectos" => CreateProyectos(),
            "Tareas" => CreateTareas(),
            "Clientes" => CreateClientes(),
            "Reportes" => CreateReportes(),
            "Configuracion" => CreateConfiguracion(),
            _ => throw new ArgumentException($"Vista desconocida: {viewName}", nameof(viewName))
        };
    }

    private static UserControl CreateDashboard()
    {
        var empleadoService = ServiceLocator.GetService<IEmpleadoService>();
        var proyectoService = ServiceLocator.GetService<IProyectoService>();
        var tareaService = ServiceLocator.GetService<ITareaService>();
        var clienteService = ServiceLocator.GetService<IClienteService>();
        var viewModel = new DashboardViewModel(empleadoService, proyectoService, tareaService, clienteService);
        return new DashboardView(viewModel);
    }

    private static UserControl CreateEmpleados()
    {
        var empleadoService = ServiceLocator.GetService<IEmpleadoService>();
        var exportService = ServiceLocator.GetService<IExportService>();
        return new EmpleadosView(new EmpleadosViewModel(empleadoService, exportService));
    }

    private static UserControl CreateProyectos()
    {
        var proyectoService = ServiceLocator.GetService<IProyectoService>();
        var exportService = ServiceLocator.GetService<IExportService>();
        return new ProyectosView(new ProyectosViewModel(proyectoService, exportService));
    }

    private static UserControl CreateTareas()
    {
        var tareaService = ServiceLocator.GetService<ITareaService>();
        var exportService = ServiceLocator.GetService<IExportService>();
        return new TareasView(new TareasViewModel(tareaService, exportService));
    }

    private static UserControl CreateClientes()
    {
        var clienteService = ServiceLocator.GetService<IClienteService>();
        var exportService = ServiceLocator.GetService<IExportService>();
        return new ClientesView(new ClientesViewModel(clienteService, exportService));
    }

    private static UserControl CreateReportes()
    {
        var reporteService = ServiceLocator.GetService<IReporteService>();
        var exportService = ServiceLocator.GetService<IExportService>();
        return new ReportesView(new ReportesViewModel(reporteService, exportService));
    }

    private static UserControl CreateConfiguracion()
    {
        var backupService = ServiceLocator.GetService<IBackupService>();
        var dbInitService = ServiceLocator.GetService<IDatabaseInitializationService>();
        var authService = ServiceLocator.GetService<IAuthenticationService>();
        return new ConfiguracionView(new ConfiguracionViewModel(backupService, dbInitService, authService));
    }
}
