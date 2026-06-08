using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de tareas.</summary>
public class TareasViewModel : ViewModelBase
{
    private readonly ITareaService _tareaService;
    private readonly IExportService _exportService;
    private IEnumerable<Tarea> _tareas = new List<Tarea>();
    private Tarea? _tareaSeleccionada;
    private EstadoTarea? _filtroEstado;

    /// <summary>Construye el ViewModel e inicia la carga de tareas.</summary>
    public TareasViewModel(ITareaService tareaService, IExportService exportService)
    {
        _tareaService = tareaService;
        _exportService = exportService;
        CargarTareasCommand = new AsyncRelayCommand(CargarTareasAsync);
        CrearTareaCommand = new RelayCommand(CrearTarea);
        EditarTareaCommand = new RelayCommand<Tarea>(EditarTarea, CanEditarTarea);
        EliminarTareaCommand = new AsyncRelayCommand<Tarea>(EliminarTareaAsync, CanEliminarTarea);
        FiltrarPorEstadoCommand = new RelayCommand<EstadoTarea?>(FiltrarPorEstado);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        
        CargarTareasCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageTareas;
    public bool CanExport => RolePermissions.CanExportData;

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
    public IAsyncRelayCommand ExportarCommand { get; }

    public event EventHandler<Tarea>? TareaCreada;
    public event EventHandler<string>? ExportacionCompletada;
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
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
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

        var resultado = MessageBox.Show(
            $"¿Está seguro de que desea eliminar la tarea '{tarea.Titulo}'?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
            return;

        IsLoading = true;
        try
        {
            await _tareaService.DeleteAsync(tarea.Id);
            await CargarTareasAsync();
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

    private bool CanEliminarTarea(Tarea? tarea) => tarea != null;

    private void FiltrarPorEstado(EstadoTarea? estado)
    {
        FiltroEstado = estado;
    }

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(_tareas, "tareas");
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
}
