using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel del panel principal con estadísticas.</summary>
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
    private int _tareasUrgentes;
    private int _tareasEnProgreso;
    private int _tareasSinFecha;
    private int _porcentajePendientes;
    private bool _tieneProyectosActivos;

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

        ProyectosActivos = new ObservableCollection<DashboardProyectoItem>();
        CargarEstadisticasCommand = new AsyncRelayCommand(CargarEstadisticasAsync);
        VerTareasCommand = new RelayCommand(() => AppNavigationService.RequestSection("Tareas"));
        VerProyectosCommand = new RelayCommand(() => AppNavigationService.RequestSection("Proyectos"));

        CargarEstadisticasCommand.ExecuteAsync(null);
    }

    public ObservableCollection<DashboardProyectoItem> ProyectosActivos { get; }

    public string SubtituloBienvenida =>
        $"Bienvenido de nuevo, {SessionService.GetUserName()}. Aquí tienes el resumen de hoy.";

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

    public int TareasUrgentes
    {
        get => _tareasUrgentes;
        set => SetProperty(ref _tareasUrgentes, value);
    }

    public int TareasEnProgreso
    {
        get => _tareasEnProgreso;
        set => SetProperty(ref _tareasEnProgreso, value);
    }

    public int TareasSinFecha
    {
        get => _tareasSinFecha;
        set => SetProperty(ref _tareasSinFecha, value);
    }

    public int PorcentajePendientes
    {
        get => _porcentajePendientes;
        set => SetProperty(ref _porcentajePendientes, value);
    }

    public bool TieneProyectosActivos
    {
        get => _tieneProyectosActivos;
        set => SetProperty(ref _tieneProyectosActivos, value);
    }

    public IAsyncRelayCommand CargarEstadisticasCommand { get; }
    public IRelayCommand VerTareasCommand { get; }
    public IRelayCommand VerProyectosCommand { get; }

    private async Task CargarEstadisticasAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var empleados = await _empleadoService.GetAllAsync();
            TotalEmpleados = empleados.Count();

            var proyectos = (await _proyectoService.GetAllAsync()).ToList();
            TotalProyectos = proyectos.Count;
            var enProgreso = proyectos.Where(p => p.Estado == EstadoProyecto.EnProgreso).ToList();
            ProyectosEnProgreso = enProgreso.Count;

            var tareas = (await _tareaService.GetAllAsync()).ToList();
            TotalTareas = tareas.Count;
            var pendientes = tareas.Where(t => t.Estado == EstadoTarea.Pendiente).ToList();
            TareasPendientes = pendientes.Count;
            TareasUrgentes = tareas.Count(t =>
                (t.Estado == EstadoTarea.Pendiente || t.Estado == EstadoTarea.EnProgreso) &&
                (t.Prioridad == PrioridadTarea.Alta || t.Prioridad == PrioridadTarea.Critica));
            TareasEnProgreso = tareas.Count(t => t.Estado == EstadoTarea.EnProgreso);
            TareasSinFecha = pendientes.Count(t => !t.FechaVencimiento.HasValue);
            PorcentajePendientes = TotalTareas > 0
                ? (int)Math.Round(100.0 * TareasPendientes / TotalTareas)
                : 0;

            var clientes = await _clienteService.GetAllAsync();
            TotalClientes = clientes.Count();

            ProyectosActivos.Clear();
            foreach (var proyecto in enProgreso.Take(4))
            {
                var tareasProyecto = tareas.Where(t => t.ProyectoId == proyecto.Id).ToList();
                var total = tareasProyecto.Count;
                var completadas = tareasProyecto.Count(t => t.Estado == EstadoTarea.Completada);
                var pct = total > 0 ? (int)Math.Round(100.0 * completadas / total) : 0;

                ProyectosActivos.Add(new DashboardProyectoItem
                {
                    Nombre = proyecto.Nombre,
                    ProgresoPorcentaje = pct
                });
            }

            TieneProyectosActivos = ProyectosActivos.Count > 0;
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
