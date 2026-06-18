using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

/// <summary>Crea vistas y ViewModels para la navegación principal.</summary>
public static class NavigationViewFactory
{
    public static UserControl CreateView(string viewName, IServiceProvider? services = null)
    {
        services ??= ServiceLocator.Provider;

        return viewName switch
        {
            "Dashboard" => CreateDashboard(services),
            "Empleados" => CreateEmpleados(services),
            "Proyectos" => CreateProyectos(services),
            "Tareas" => CreateTareas(services),
            "Clientes" => CreateClientes(services),
            "Reportes" => CreateReportes(services),
            "Configuracion" => CreateConfiguracion(services),
            "Perfil" => CreatePerfil(),
            _ => throw new ArgumentException($"Vista desconocida: {viewName}", nameof(viewName))
        };
    }

    private static UserControl CreateDashboard(IServiceProvider services)
    {
        var viewModel = new DashboardViewModel(
            services.GetRequiredService<IEmpleadoService>(),
            services.GetRequiredService<IProyectoService>(),
            services.GetRequiredService<ITareaService>(),
            services.GetRequiredService<IClienteService>());
        return new DashboardView(viewModel);
    }

    private static UserControl CreateEmpleados(IServiceProvider services)
    {
        var empleadoService = services.GetRequiredService<IEmpleadoService>();
        var exportService = services.GetRequiredService<IExportService>();
        var usuarioService = services.GetRequiredService<IUsuarioService>();
        return new EmpleadosView(new EmpleadosViewModel(empleadoService, exportService), empleadoService, usuarioService);
    }

    private static UserControl CreateProyectos(IServiceProvider services)
    {
        var proyectoService = services.GetRequiredService<IProyectoService>();
        var clienteService = services.GetRequiredService<IClienteService>();
        var empleadoService = services.GetRequiredService<IEmpleadoService>();
        var exportService = services.GetRequiredService<IExportService>();
        return new ProyectosView(
            new ProyectosViewModel(proyectoService, exportService),
            proyectoService,
            clienteService,
            empleadoService);
    }

    private static UserControl CreateTareas(IServiceProvider services)
    {
        var tareaService = services.GetRequiredService<ITareaService>();
        var proyectoService = services.GetRequiredService<IProyectoService>();
        var empleadoService = services.GetRequiredService<IEmpleadoService>();
        var exportService = services.GetRequiredService<IExportService>();
        return new TareasView(
            new TareasViewModel(tareaService, exportService),
            tareaService,
            proyectoService,
            empleadoService);
    }

    private static UserControl CreateClientes(IServiceProvider services)
    {
        var clienteService = services.GetRequiredService<IClienteService>();
        var exportService = services.GetRequiredService<IExportService>();
        return new ClientesView(new ClientesViewModel(clienteService, exportService), clienteService);
    }

    private static UserControl CreateReportes(IServiceProvider services)
    {
        var reporteService = services.GetRequiredService<IReporteService>();
        var exportService = services.GetRequiredService<IExportService>();
        return new ReportesView(new ReportesViewModel(reporteService, exportService));
    }

    private static UserControl CreateConfiguracion(IServiceProvider services)
    {
        return new ConfiguracionView(new ConfiguracionViewModel(
            services.GetRequiredService<IBackupService>(),
            services.GetRequiredService<IDatabaseInitializationService>(),
            services.GetRequiredService<IAuthenticationService>()));
    }

    private static UserControl CreatePerfil() =>
        new ProfileView(new ProfileViewModel());
}
