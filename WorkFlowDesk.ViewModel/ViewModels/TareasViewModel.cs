using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Models;
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
    private IEnumerable<Tarea> _tareasFiltradas = new List<Tarea>();
    private Tarea? _tareaSeleccionada;
    private FiltroOpcion<EstadoTarea?>? _filtroEstadoSeleccionado;
    private string _textoBusqueda = string.Empty;

    public IReadOnlyList<FiltroOpcion<EstadoTarea?>> FiltrosEstado { get; } = new List<FiltroOpcion<EstadoTarea?>>
    {
        new() { Etiqueta = "Todas", Valor = null },
        new() { Etiqueta = "Pendiente", Valor = EstadoTarea.Pendiente },
        new() { Etiqueta = "En progreso", Valor = EstadoTarea.EnProgreso },
        new() { Etiqueta = "En revisión", Valor = EstadoTarea.EnRevision },
        new() { Etiqueta = "Completada", Valor = EstadoTarea.Completada },
        new() { Etiqueta = "Cancelada", Valor = EstadoTarea.Cancelada }
    };

    /// <summary>Construye el ViewModel e inicia la carga de tareas.</summary>
    public TareasViewModel(ITareaService tareaService, IExportService exportService)
    {
        _tareaService = tareaService;
        _exportService = exportService;
        _filtroEstadoSeleccionado = FiltrosEstado[0];
        CargarTareasCommand = new AsyncRelayCommand(CargarTareasAsync);
        CrearTareaCommand = new RelayCommand(CrearTarea);
        EditarTareaCommand = new RelayCommand<Tarea>(EditarTarea, CanEditarTarea);
        EliminarTareaCommand = new AsyncRelayCommand<Tarea>(EliminarTareaAsync, CanEliminarTarea);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        
        CargarTareasCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageTareas;
    public bool CanExport => RolePermissions.CanExportData;

    public IEnumerable<Tarea> Tareas
    {
        get => _tareasFiltradas;
        set => SetProperty(ref _tareasFiltradas, value);
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            SetProperty(ref _textoBusqueda, value);
            FiltrarTareas();
        }
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

    public FiltroOpcion<EstadoTarea?>? FiltroEstadoSeleccionado
    {
        get => _filtroEstadoSeleccionado;
        set
        {
            if (SetProperty(ref _filtroEstadoSeleccionado, value))
            {
                CargarTareasCommand.ExecuteAsync(null);
            }
        }
    }

    public IAsyncRelayCommand CargarTareasCommand { get; }
    public IRelayCommand CrearTareaCommand { get; }
    public IRelayCommand<Tarea> EditarTareaCommand { get; }
    public IAsyncRelayCommand<Tarea> EliminarTareaCommand { get; }
    public IAsyncRelayCommand ExportarCommand { get; }

    public event EventHandler<Tarea>? TareaCreada;
    public event EventHandler<string>? ExportacionCompletada;
    public event EventHandler<Tarea>? TareaEditada;

    private async Task CargarTareasAsync()
    {
        IsLoading = true;
        try
        {
            var filtro = FiltroEstadoSeleccionado?.Valor;
            if (filtro.HasValue)
            {
                _tareas = await _tareaService.GetByEstadoAsync(filtro.Value);
            }
            else
            {
                _tareas = await _tareaService.GetAllAsync();
            }

            FiltrarTareas();
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

        if (!SolicitarConfirmacion(
                $"¿Está seguro de que desea eliminar la tarea '{tarea.Titulo}'?",
                "Confirmar eliminación"))
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

    private void FiltrarTareas()
    {
        if (string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            Tareas = _tareas;
            return;
        }

        Tareas = SearchHelper.FilterByText(
            _tareas,
            TextoBusqueda,
            t => t.Titulo,
            t => t.Descripcion);
    }

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(_tareasFiltradas, "tareas");
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
