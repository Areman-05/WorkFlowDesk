using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de reportes y estadísticas.</summary>
public class ReportesViewModel : ViewModelBase
{
    private readonly IReporteService _reporteService;
    private readonly IExportService _exportService;
    private Dictionary<string, int> _estadisticasEmpleados = new();
    private Dictionary<string, int> _estadisticasProyectos = new();
    private Dictionary<string, int> _estadisticasTareas = new();
    private Dictionary<string, int> _estadisticasClientes = new();

    /// <summary>Construye el ViewModel e inicia la carga de reportes.</summary>
    public ReportesViewModel(IReporteService reporteService, IExportService exportService)
    {
        _reporteService = reporteService;
        _exportService = exportService;
        CargarReportesCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CargarReportesAsync);
        ExportarCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(ExportarAsync);
        CargarReportesCommand.ExecuteAsync(null);
    }

    public bool CanExport => RolePermissions.CanExportData;

    public Dictionary<string, int> EstadisticasEmpleados
    {
        get => _estadisticasEmpleados;
        set => SetProperty(ref _estadisticasEmpleados, value);
    }

    public Dictionary<string, int> EstadisticasProyectos
    {
        get => _estadisticasProyectos;
        set => SetProperty(ref _estadisticasProyectos, value);
    }

    public Dictionary<string, int> EstadisticasTareas
    {
        get => _estadisticasTareas;
        set => SetProperty(ref _estadisticasTareas, value);
    }

    public Dictionary<string, int> EstadisticasClientes
    {
        get => _estadisticasClientes;
        set => SetProperty(ref _estadisticasClientes, value);
    }

    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CargarReportesCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand ExportarCommand { get; }

    public event EventHandler<string>? ExportacionCompletada;

    private async Task CargarReportesAsync()
    {
        IsLoading = true;
        try
        {
            EstadisticasEmpleados = await _reporteService.ObtenerEstadisticasEmpleadosAsync();
            EstadisticasProyectos = await _reporteService.ObtenerEstadisticasProyectosAsync();
            EstadisticasTareas = await _reporteService.ObtenerEstadisticasTareasAsync();
            EstadisticasClientes = await _reporteService.ObtenerEstadisticasClientesAsync();
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var filas = ConstruirFilasExportacion();
            var path = await _exportService.ExportToCsvAsync(filas, "reportes");
            ExportacionCompletada?.Invoke(this, path);
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private List<ReporteExportRow> ConstruirFilasExportacion()
    {
        var filas = new List<ReporteExportRow>();
        filas.AddRange(EstadisticasEmpleados.Select(kv => new ReporteExportRow
        {
            Seccion = "Empleados",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        filas.AddRange(EstadisticasProyectos.Select(kv => new ReporteExportRow
        {
            Seccion = "Proyectos",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        filas.AddRange(EstadisticasTareas.Select(kv => new ReporteExportRow
        {
            Seccion = "Tareas",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        filas.AddRange(EstadisticasClientes.Select(kv => new ReporteExportRow
        {
            Seccion = "Clientes",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        return filas;
    }
}
