using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de proyectos.</summary>
public class ProyectosViewModel : ListViewModelBase
{
    private readonly IProyectoService _proyectoService;
    private readonly IExportService _exportService;
    private readonly PaginationHelper _paginacion = new();
    private IEnumerable<Proyecto> _proyectos = new List<Proyecto>();
    private IEnumerable<Proyecto> _proyectosFiltrados = new List<Proyecto>();
    private List<Proyecto> _resultadoFiltrado = new();
    private Proyecto? _proyectoSeleccionado;
    private string _textoBusqueda = string.Empty;

    /// <summary>Construye el ViewModel e inicia la carga de proyectos.</summary>
    public ProyectosViewModel(IProyectoService proyectoService, IExportService exportService)
    {
        _proyectoService = proyectoService;
        _exportService = exportService;
        CargarProyectosCommand = new AsyncRelayCommand(CargarProyectosAsync);
        CrearProyectoCommand = new RelayCommand(CrearProyecto);
        EditarProyectoCommand = new RelayCommand<Proyecto>(EditarProyecto, CanEditarProyecto);
        EliminarProyectoCommand = new AsyncRelayCommand<Proyecto>(EliminarProyectoAsync, CanEliminarProyecto);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        PaginaAnteriorCommand = new RelayCommand(IrPaginaAnterior, () => _paginacion.TienePaginaAnterior);
        PaginaSiguienteCommand = new RelayCommand(IrPaginaSiguiente, () => _paginacion.TienePaginaSiguiente);
        
        CargarProyectosCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageProyectos;
    public string InfoPaginacion => _paginacion.Resumen;
    public bool CanExport => RolePermissions.CanExportData;

    public IEnumerable<Proyecto> Proyectos
    {
        get => _proyectosFiltrados;
        set
        {
            SetProperty(ref _proyectosFiltrados, value);
            NotificarEstadoLista();
        }
    }

    protected override int ObtenerCantidadElementos() => _proyectosFiltrados.Count();

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            SetProperty(ref _textoBusqueda, value);
            FiltrarProyectos();
        }
    }

    public Proyecto? ProyectoSeleccionado
    {
        get => _proyectoSeleccionado;
        set
        {
            SetProperty(ref _proyectoSeleccionado, value);
            EditarProyectoCommand.NotifyCanExecuteChanged();
            EliminarProyectoCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand CargarProyectosCommand { get; }
    public IRelayCommand CrearProyectoCommand { get; }
    public IRelayCommand<Proyecto> EditarProyectoCommand { get; }
    public IAsyncRelayCommand<Proyecto> EliminarProyectoCommand { get; }
    public IAsyncRelayCommand ExportarCommand { get; }
    public IRelayCommand PaginaAnteriorCommand { get; }
    public IRelayCommand PaginaSiguienteCommand { get; }

    public event EventHandler<Proyecto>? ProyectoCreado;
    public event EventHandler<Proyecto>? ProyectoEditado;
    public event EventHandler<string>? ExportacionCompletada;

    private async Task CargarProyectosAsync()
    {
        IsLoading = true;
        NotificarEstadoLista();
        try
        {
            _proyectos = await _proyectoService.GetAllAsync();
            FiltrarProyectos();
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

    private void CrearProyecto()
    {
        var nuevoProyecto = new Proyecto
        {
            Estado = EstadoProyecto.Planificacion,
            FechaInicio = DateTime.Now
        };
        ProyectoCreado?.Invoke(this, nuevoProyecto);
    }

    private void EditarProyecto(Proyecto? proyecto)
    {
        if (proyecto != null)
        {
            ProyectoEditado?.Invoke(this, proyecto);
        }
    }

    private bool CanEditarProyecto(Proyecto? proyecto) => proyecto != null;

    private async Task EliminarProyectoAsync(Proyecto? proyecto)
    {
        if (proyecto == null) return;

        if (!SolicitarConfirmacion(
                $"¿Está seguro de que desea eliminar el proyecto '{proyecto.Nombre}'?",
                "Confirmar eliminación"))
            return;

        IsLoading = true;
        try
        {
            await _proyectoService.DeleteAsync(proyecto.Id);
            await CargarProyectosAsync();
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

    private bool CanEliminarProyecto(Proyecto? proyecto) => proyecto != null;

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(_resultadoFiltrado, "proyectos");
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

    private void FiltrarProyectos()
    {
        _resultadoFiltrado = string.IsNullOrWhiteSpace(TextoBusqueda)
            ? _proyectos.ToList()
            : SearchHelper.FilterByText(
                _proyectos,
                TextoBusqueda,
                p => p.Nombre,
                p => p.Descripcion).ToList();

        _paginacion.Reiniciar();
        AplicarPaginacion();
    }

    private void AplicarPaginacion()
    {
        _paginacion.TamañoPagina = Math.Max(1, AppConfig.Settings.DefaultPageSize);
        Proyectos = _paginacion.Aplicar(_resultadoFiltrado).ToList();
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
}
