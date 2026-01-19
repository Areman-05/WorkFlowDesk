using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class TareasViewModel : ViewModelBase
{
    private readonly ITareaService _tareaService;
    private IEnumerable<Tarea> _tareas = new List<Tarea>();
    private Tarea? _tareaSeleccionada;
    private EstadoTarea? _filtroEstado;

    public TareasViewModel(ITareaService tareaService)
    {
        _tareaService = tareaService;
        CargarTareasCommand = new AsyncRelayCommand(CargarTareasAsync);
        CrearTareaCommand = new RelayCommand(CrearTarea);
        EditarTareaCommand = new RelayCommand<Tarea>(EditarTarea, CanEditarTarea);
        EliminarTareaCommand = new AsyncRelayCommand<Tarea>(EliminarTareaAsync, CanEliminarTarea);
        FiltrarPorEstadoCommand = new RelayCommand<EstadoTarea?>(FiltrarPorEstado);
        
        CargarTareasCommand.ExecuteAsync(null);
    }

    public IEnumerable<Tarea> Tareas
    {
        get => _tareas;
        set => SetProperty(ref _tareas, value);
    }

    public Tarea? TareaSeleccionada
    {
        get => _tareaSeleccionada;
        set
        {
            SetProperty(ref _tareaSeleccionada, value);
            EditarTareaCommand.NotifyCanExecuteChanged();
            EliminarTareaCommand.NotifyCanExecuteChanged();
        }
    }

    public EstadoTarea? FiltroEstado
    {
        get => _filtroEstado;
        set
        {
            SetProperty(ref _filtroEstado, value);
            CargarTareasCommand.ExecuteAsync(null);
        }
    }

    public IAsyncRelayCommand CargarTareasCommand { get; }
    public IRelayCommand CrearTareaCommand { get; }
    public IRelayCommand<Tarea> EditarTareaCommand { get; }
    public IAsyncRelayCommand<Tarea> EliminarTareaCommand { get; }
    public IRelayCommand<EstadoTarea?> FiltrarPorEstadoCommand { get; }

    public event EventHandler<Tarea>? TareaCreada;
    public event EventHandler<Tarea>? TareaEditada;

    private async Task CargarTareasAsync()
    {
        IsLoading = true;
        try
        {
            if (FiltroEstado.HasValue)
            {
                Tareas = await _tareaService.GetByEstadoAsync(FiltroEstado.Value);
            }
            else
            {
                Tareas = await _tareaService.GetAllAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar tareas: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CrearTarea()
    {
        var nuevaTarea = new Tarea
        {
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Media,
            FechaCreacion = DateTime.Now
        };
        TareaCreada?.Invoke(this, nuevaTarea);
    }

    private void EditarTarea(Tarea? tarea)
    {
        if (tarea != null)
        {
            TareaEditada?.Invoke(this, tarea);
        }
    }

    private bool CanEditarTarea(Tarea? tarea) => tarea != null;

    private async Task EliminarTareaAsync(Tarea? tarea)
    {
        if (tarea == null) return;

        IsLoading = true;
        try
        {
            await _tareaService.DeleteAsync(tarea.Id);
            await CargarTareasAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al eliminar tarea: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanEliminarTarea(Tarea? tarea) => tarea != null;

    private void FiltrarPorEstado(EstadoTarea? estado)
    {
        FiltroEstado = estado;
    }
}
