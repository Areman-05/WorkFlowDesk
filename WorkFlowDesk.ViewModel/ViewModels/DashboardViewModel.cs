using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IProyectoService _proyectoService;
    private readonly ITareaService _tareaService;
    private readonly IClienteService _clienteService;

    private int _totalEmpleados;
    private int _totalProyectos;
    private int _totalTareas;
    private int _totalClientes;
    private int _tareasPendientes;
    private int _proyectosEnProgreso;

    public DashboardViewModel(
        IEmpleadoService empleadoService,
        IProyectoService proyectoService,
        ITareaService tareaService,
        IClienteService clienteService)
    {
        _empleadoService = empleadoService;
        _proyectoService = proyectoService;
        _tareaService = tareaService;
        _clienteService = clienteService;

        CargarEstadisticasCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CargarEstadisticasAsync);
        CargarEstadisticasCommand.ExecuteAsync(null);
    }

    public int TotalEmpleados
    {
        get => _totalEmpleados;
        set => SetProperty(ref _totalEmpleados, value);
    }

    public int TotalProyectos
    {
        get => _totalProyectos;
        set => SetProperty(ref _totalProyectos, value);
    }

    public int TotalTareas
    {
        get => _totalTareas;
        set => SetProperty(ref _totalTareas, value);
    }

    public int TotalClientes
    {
        get => _totalClientes;
        set => SetProperty(ref _totalClientes, value);
    }

    public int TareasPendientes
    {
        get => _tareasPendientes;
        set => SetProperty(ref _tareasPendientes, value);
    }

    public int ProyectosEnProgreso
    {
        get => _proyectosEnProgreso;
        set => SetProperty(ref _proyectosEnProgreso, value);
    }

    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CargarEstadisticasCommand { get; }

    private async Task CargarEstadisticasAsync()
    {
        IsLoading = true;
        try
        {
            var empleados = await _empleadoService.GetAllAsync();
            TotalEmpleados = empleados.Count();

            var proyectos = await _proyectoService.GetAllAsync();
            TotalProyectos = proyectos.Count();
            ProyectosEnProgreso = proyectos.Count(p => p.Estado == Domain.Entities.EstadoProyecto.EnProgreso);

            var tareas = await _tareaService.GetAllAsync();
            TotalTareas = tareas.Count();
            TareasPendientes = tareas.Count(t => t.Estado == Domain.Entities.EstadoTarea.Pendiente);

            var clientes = await _clienteService.GetAllAsync();
            TotalClientes = clientes.Count();
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
