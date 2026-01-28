using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class ReportesViewModel : ViewModelBase
{
    private readonly IReporteService _reporteService;
    private Dictionary<string, int> _estadisticasEmpleados = new();
    private Dictionary<string, int> _estadisticasProyectos = new();
    private Dictionary<string, int> _estadisticasTareas = new();
    private Dictionary<string, int> _estadisticasClientes = new();

    public ReportesViewModel(IReporteService reporteService)
    {
        _reporteService = reporteService;
        CargarReportesCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CargarReportesAsync);
        CargarReportesCommand.ExecuteAsync(null);
    }

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
}
