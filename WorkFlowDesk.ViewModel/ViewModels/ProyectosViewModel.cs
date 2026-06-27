using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de proyectos.</summary>
public class ProyectosViewModel : ListViewModelBase, ISearchableViewModel, IListToolbarProvider
{
    private readonly IProyectoService _proyectoService;
    private readonly IExportService _exportService;
    private readonly ITareaService _tareaService;
    private readonly PaginationHelper _paginacion = new();
    private IEnumerable<Proyecto> _proyectos = new List<Proyecto>();
    private IEnumerable<ProyectoListItem> _proyectosFiltrados = new List<ProyectoListItem>();
    private List<Proyecto> _resultadoFiltrado = new();
    private Dictionary<int, (int Completadas, int Total)> _progresoPorProyecto = new();
    private ProyectoListItem? _proyectoSeleccionado;
    private string _textoBusqueda = string.Empty;
    private string _filtroActivo = "Todos";
    private int _totalProyectos;
    private int _enProgreso;
    private int _proximosVencer;
    private int _retrasados;

    public ProyectosViewModel(
        IProyectoService proyectoService,
        IExportService exportService,
        ITareaService tareaService)
    {
        _proyectoService = proyectoService;
        _exportService = exportService;
        _tareaService = tareaService;

        CargarProyectosCommand = new AsyncRelayCommand(CargarProyectosAsync);
        CrearProyectoCommand = new RelayCommand(CrearProyecto);
        EditarProyectoCommand = new RelayCommand<ProyectoListItem>(EditarProyecto, CanEditarProyecto);
        EliminarProyectoCommand = new AsyncRelayCommand<ProyectoListItem>(EliminarProyectoAsync, CanEliminarProyecto);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        PaginaAnteriorCommand = new RelayCommand(IrPaginaAnterior, () => _paginacion.TienePaginaAnterior);
        PaginaSiguienteCommand = new RelayCommand(IrPaginaSiguiente, () => _paginacion.TienePaginaSiguiente);
        FiltrarTodosCommand = new RelayCommand(() => AplicarFiltroEstado("Todos"));
        FiltrarActivosCommand = new RelayCommand(() => AplicarFiltroEstado("Activos"));
        FiltrarCompletadosCommand = new RelayCommand(() => AplicarFiltroEstado("Completados"));

        CargarProyectosCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageProyectos;
    public bool CanExport => RolePermissions.CanExportData;

    public string InfoPaginacion => _paginacion.TotalItems == 0
        ? "Sin proyectos"
        : $"{_paginacion.TotalItems} proyectos";

    public int TotalProyectos
    {
        get => _totalProyectos;
        private set => SetProperty(ref _totalProyectos, value);
    }

    public int EnProgreso
    {
        get => _enProgreso;
        private set => SetProperty(ref _enProgreso, value);
    }

    public int ProximosVencer
    {
        get => _proximosVencer;
        private set => SetProperty(ref _proximosVencer, value);
    }

    public int Retrasados
    {
        get => _retrasados;
        private set => SetProperty(ref _retrasados, value);
    }

    public bool FiltroTodosActivo => _filtroActivo == "Todos";
    public bool FiltroActivosActivo => _filtroActivo == "Activos";
    public bool FiltroCompletadosActivo => _filtroActivo == "Completados";

    public IEnumerable<ProyectoListItem> Proyectos
    {
        get => _proyectosFiltrados;
        private set
        {
            SetProperty(ref _proyectosFiltrados, value);
            NotificarEstadoLista();
            OnPropertyChanged(nameof(InfoPaginacion));
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

    public ProyectoListItem? ProyectoSeleccionado
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
    public IRelayCommand<ProyectoListItem> EditarProyectoCommand { get; }
    public IAsyncRelayCommand<ProyectoListItem> EliminarProyectoCommand { get; }
    public IAsyncRelayCommand ExportarCommand { get; }
    public IRelayCommand PaginaAnteriorCommand { get; }
    public IRelayCommand PaginaSiguienteCommand { get; }
    public IRelayCommand FiltrarTodosCommand { get; }
    public IRelayCommand FiltrarActivosCommand { get; }
    public IRelayCommand FiltrarCompletadosCommand { get; }

    public event EventHandler<Proyecto>? ProyectoCreado;
    public event EventHandler<Proyecto>? ProyectoEditado;
    public event EventHandler<string>? ExportacionCompletada;

    public bool ToolbarExportVisible => CanExport;
    public bool ToolbarCreateVisible => CanManage;
    public string ToolbarCreateLabel => "Nuevo proyecto";
    public IAsyncRelayCommand? ToolbarExportCommand => ExportarCommand;
    public IRelayCommand? ToolbarCreateCommand => CrearProyectoCommand;

    private async Task CargarProyectosAsync()
    {
        IsLoading = true;
        NotificarEstadoLista();
        try
        {
            _proyectos = (await _proyectoService.GetAllAsync()).ToList();
            await CargarProgresoTareasAsync();
            ActualizarEstadisticas();
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

    private async Task CargarProgresoTareasAsync()
    {
        var tareas = (await _tareaService.GetAllAsync()).ToList();
        _progresoPorProyecto = tareas
            .Where(t => t.ProyectoId.HasValue)
            .GroupBy(t => t.ProyectoId!.Value)
            .ToDictionary(
                g => g.Key,
                g => (
                    Completadas: g.Count(t => t.Estado == EstadoTarea.Completada),
                    Total: g.Count()));
    }

    private void ActualizarEstadisticas()
    {
        var lista = _proyectos.ToList();
        var hoy = DateTime.Today;
        var limite = hoy.AddDays(7);

        TotalProyectos = lista.Count;
        EnProgreso = lista.Count(p => p.Estado == EstadoProyecto.EnProgreso);
        ProximosVencer = lista.Count(p =>
            p.Estado is not (EstadoProyecto.Completado or EstadoProyecto.Cancelado) &&
            p.FechaFin.HasValue &&
            p.FechaFin.Value.Date >= hoy &&
            p.FechaFin.Value.Date <= limite);
        Retrasados = lista.Count(p =>
            p.Estado is not (EstadoProyecto.Completado or EstadoProyecto.Cancelado) &&
            ((p.FechaFin.HasValue && p.FechaFin.Value.Date < hoy) ||
             p.Estado == EstadoProyecto.EnPausa));
    }

    private void AplicarFiltroEstado(string filtro)
    {
        _filtroActivo = filtro;
        OnPropertyChanged(nameof(FiltroTodosActivo));
        OnPropertyChanged(nameof(FiltroActivosActivo));
        OnPropertyChanged(nameof(FiltroCompletadosActivo));
        FiltrarProyectos();
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

    private void EditarProyecto(ProyectoListItem? item)
    {
        if (item != null)
            ProyectoEditado?.Invoke(this, item.Proyecto);
    }

    private bool CanEditarProyecto(ProyectoListItem? item) => item != null;

    private async Task EliminarProyectoAsync(ProyectoListItem? item)
    {
        if (item == null) return;

        var proyecto = item.Proyecto;
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

    private bool CanEliminarProyecto(ProyectoListItem? item) => item != null;

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
        var consulta = _proyectos.AsEnumerable();

        consulta = _filtroActivo switch
        {
            "Activos" => consulta.Where(p => p.Estado is not (EstadoProyecto.Completado or EstadoProyecto.Cancelado)),
            "Completados" => consulta.Where(p => p.Estado == EstadoProyecto.Completado),
            _ => consulta
        };

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            consulta = SearchHelper.FilterByText(
                consulta,
                TextoBusqueda,
                p => p.Nombre,
                p => p.Descripcion,
                p => p.Cliente?.Nombre ?? string.Empty);
        }

        _resultadoFiltrado = consulta.ToList();
        _paginacion.Reiniciar();
        AplicarPaginacion();
    }

    private void AplicarPaginacion()
    {
        _paginacion.TamañoPagina = Math.Max(1, _resultadoFiltrado.Count);
        var pagina = _paginacion.Aplicar(_resultadoFiltrado).Select(MapToListItem).ToList();
        Proyectos = pagina;
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

    private ProyectoListItem MapToListItem(Proyecto proyecto)
    {
        var progreso = CalcularProgreso(proyecto);
        var (estadoTexto, estadoFondo, estadoColor) = proyecto.Estado switch
        {
            EstadoProyecto.EnProgreso => ("En progreso", "#DBEAFE", "#1D4ED8"),
            EstadoProyecto.Completado => ("Completada", "#DCFCE7", "#15803D"),
            EstadoProyecto.Planificacion => ("Pendiente", "#FEF3C7", "#B45309"),
            EstadoProyecto.EnPausa => ("En pausa", "#FFF4E6", "#F76707"),
            EstadoProyecto.Cancelado => ("Cancelado", "#FFEBEB", "#BA1A1A"),
            _ => ("Pendiente", "#FEF3C7", "#B45309")
        };

        return new ProyectoListItem
        {
            Proyecto = proyecto,
            Iniciales = GetIniciales(proyecto.Nombre),
            CodigoId = $"ID: PRJ-{proyecto.Id:D4}",
            NombreCliente = string.IsNullOrWhiteSpace(proyecto.Cliente?.Nombre) ? "—" : proyecto.Cliente!.Nombre,
            FechaInicioTexto = proyecto.FechaInicio.ToString("dd MMM, yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")),
            EstadoTexto = estadoTexto,
            EstadoFondo = estadoFondo,
            EstadoTextoColor = estadoColor,
            Progreso = progreso,
            ProgresoCompleto = progreso >= 100
        };
    }

    private int CalcularProgreso(Proyecto proyecto)
    {
        if (proyecto.Estado == EstadoProyecto.Completado)
            return 100;

        if (_progresoPorProyecto.TryGetValue(proyecto.Id, out var datos) && datos.Total > 0)
            return (int)Math.Round(100.0 * datos.Completadas / datos.Total);

        return proyecto.Estado switch
        {
            EstadoProyecto.Planificacion => 10,
            EstadoProyecto.EnProgreso => 50,
            EstadoProyecto.EnPausa => 30,
            _ => 0
        };
    }

    private static string GetIniciales(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "PR";

        var parts = nombre.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();

        return nombre.Length >= 2
            ? nombre[..2].ToUpperInvariant()
            : nombre[0].ToString().ToUpperInvariant();
    }
}
