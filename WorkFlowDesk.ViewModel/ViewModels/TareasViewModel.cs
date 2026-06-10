using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de tareas.</summary>
public class TareasViewModel : ListViewModelBase
{
    private readonly ITareaService _tareaService;
    private readonly IExportService _exportService;
    private readonly PaginationHelper _paginacion = new();
    private IEnumerable<Tarea> _tareas = new List<Tarea>();
    private IEnumerable<Tarea> _tareasFiltradas = new List<Tarea>();
    private List<Tarea> _resultadoFiltrado = new();
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
        PaginaAnteriorCommand = new RelayCommand(IrPaginaAnterior, () => _paginacion.TienePaginaAnterior);
        PaginaSiguienteCommand = new RelayCommand(IrPaginaSiguiente, () => _paginacion.TienePaginaSiguiente);
        
        CargarTareasCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageTareas;
    public string InfoPaginacion => _paginacion.Resumen;
    public bool CanExport => RolePermissions.CanExportData;

    public IEnumerable<Tarea> Tareas
    {
        get => _tareasFiltradas;
        set
        {
            SetProperty(ref _tareasFiltradas, value);
            NotificarEstadoLista();
        }
    }

    protected override int ObtenerCantidadElementos() => _tareasFiltradas.Count();

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
    public IRelayCommand PaginaAnteriorCommand { get; }
    public IRelayCommand PaginaSiguienteCommand { get; }

    public event EventHandler<Tarea>? TareaCreada;
    public event EventHandler<string>? ExportacionCompletada;
    public event EventHandler<Tarea>? TareaEditada;

    private async Task CargarTareasAsync()
    {
        IsLoading = true;
        NotificarEstadoLista();
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
            NotificarEstadoLista();
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
        _resultadoFiltrado = string.IsNullOrWhiteSpace(TextoBusqueda)
            ? _tareas.ToList()
            : SearchHelper.FilterByText(
                _tareas,
                TextoBusqueda,
                t => t.Titulo,
                t => t.Descripcion).ToList();

        _paginacion.Reiniciar();
        AplicarPaginacion();
    }

    private void AplicarPaginacion()
    {
        _paginacion.TamañoPagina = Math.Max(1, AppConfig.Settings.DefaultPageSize);
        Tareas = _paginacion.Aplicar(_resultadoFiltrado).ToList();
        OnPropertyChanged(nameof(InfoPaginacion));
        PaginaAnteriorCommand.NotifyCanExecuteChanged();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
    }

    private void IrPaginaAnterior()
    {
        _paginacion.PaginaAnterior();
        AplicarPaginacion();
    }

    private void IrPaginaSiguiente()
    {
        _paginacion.PaginaSiguiente();
        AplicarPaginacion();
    }

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(_resultadoFiltrado, "tareas");
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
