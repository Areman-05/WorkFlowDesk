using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de tareas.</summary>
public class TareasViewModel : ListViewModelBase, ISearchableViewModel, IListToolbarProvider
{
    private static readonly (EstadoTarea Estado, string Titulo, EstadoTarea? Siguiente, string? TextoMover)[] DefinicionesKanban =
    [
        (EstadoTarea.Pendiente, "Pendiente", EstadoTarea.EnProgreso, "→ En progreso"),
        (EstadoTarea.EnProgreso, "En progreso", EstadoTarea.EnRevision, "→ En revisión"),
        (EstadoTarea.EnRevision, "En revisión", EstadoTarea.Completada, "→ Completada"),
        (EstadoTarea.Completada, "Completada", null, null)
    ];

    private readonly ITareaService _tareaService;
    private readonly IExportService _exportService;
    private readonly ITareaExtensionService _tareaExtension;
    private readonly PaginationHelper _paginacion = new();
    private readonly ObservableCollection<KanbanColumnItem> _columnasKanban = new();
    private readonly ObservableCollection<CalendarioDiaItem> _diasCalendario = new();
    private DateTime _mesCalendario = DateTime.Today;
    private string _modoVista = "Lista";
    private string _mesAnioTexto = string.Empty;
    private IEnumerable<Tarea> _tareas = new List<Tarea>();
    private IEnumerable<TareaListItem> _tareasFiltradas = new List<TareaListItem>();
    private List<Tarea> _resultadoFiltrado = new();
    private TareaListItem? _tareaSeleccionada;
    private string _textoBusqueda = string.Empty;
    private string _filtroActivo = "Todas";
    private int _totalTareas;
    private int _pendientes;
    private int _enCurso;
    private int _completadas;
    private int _rendimientoPorcentaje;
    private string _rendimientoVariacion = string.Empty;
    private int? _proyectoFiltroId;
    private string _proyectoFiltroNombre = string.Empty;
    private IReadOnlyList<ActividadDiaItem> _actividadSemanal = Array.Empty<ActividadDiaItem>();

    public TareasViewModel(
        ITareaService tareaService,
        IExportService exportService,
        ITareaExtensionService tareaExtension)
    {
        _tareaService = tareaService;
        _exportService = exportService;
        _tareaExtension = tareaExtension;

        CargarTareasCommand = new AsyncRelayCommand(CargarTareasAsync);
        CrearTareaCommand = new RelayCommand(CrearTarea);
        EditarTareaCommand = new RelayCommand<TareaListItem>(EditarTarea, CanEditarTarea);
        EliminarTareaCommand = new AsyncRelayCommand<TareaListItem>(EliminarTareaAsync, CanEliminarTarea);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        PaginaAnteriorCommand = new RelayCommand(IrPaginaAnterior, () => _paginacion.TienePaginaAnterior);
        PaginaSiguienteCommand = new RelayCommand(IrPaginaSiguiente, () => _paginacion.TienePaginaSiguiente);
        FiltrarTodasCommand = new RelayCommand(() => AplicarFiltroEstado("Todas"));
        FiltrarPendientesCommand = new RelayCommand(() => AplicarFiltroEstado("Pendientes"));
        FiltrarEnCursoCommand = new RelayCommand(() => AplicarFiltroEstado("EnCurso"));
        FiltrarCompletadasCommand = new RelayCommand(() => AplicarFiltroEstado("Completadas"));
        VerListaCommand = new RelayCommand(() => CambiarModoVista("Lista"));
        VerKanbanCommand = new RelayCommand(() => CambiarModoVista("Kanban"));
        VerCalendarioCommand = new RelayCommand(() => CambiarModoVista("Calendario"));
        MoverKanbanCommand = new AsyncRelayCommand<KanbanMoveRequest>(MoverKanbanAsync, CanMoverKanban);
        MesAnteriorCommand = new RelayCommand(IrMesAnterior);
        MesSiguienteCommand = new RelayCommand(IrMesSiguiente);

        ActualizarMesAnioTexto();

        if (AppNavigationService.PendingProyectoFiltroId is int proyectoId)
        {
            _proyectoFiltroId = proyectoId;
            AppNavigationService.PendingProyectoFiltroId = null;
        }

        CargarTareasCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageTareas;
    public bool CanExport => RolePermissions.CanExportData;

    public string InfoPaginacion => _paginacion.TotalItems == 0
        ? "Sin tareas"
        : $"{_paginacion.TotalItems} tareas";

    public int TotalTareas
    {
        get => _totalTareas;
        private set => SetProperty(ref _totalTareas, value);
    }

    public int Pendientes
    {
        get => _pendientes;
        private set => SetProperty(ref _pendientes, value);
    }

    public int EnCurso
    {
        get => _enCurso;
        private set => SetProperty(ref _enCurso, value);
    }

    public int Completadas
    {
        get => _completadas;
        private set => SetProperty(ref _completadas, value);
    }

    public int RendimientoPorcentaje
    {
        get => _rendimientoPorcentaje;
        private set
        {
            if (SetProperty(ref _rendimientoPorcentaje, value))
                OnPropertyChanged(nameof(RendimientoArcoOffset));
        }
    }

    public double RendimientoArcoOffset => 251.2 * (1 - RendimientoPorcentaje / 100.0);

    public string RendimientoVariacion
    {
        get => _rendimientoVariacion;
        private set => SetProperty(ref _rendimientoVariacion, value);
    }

    public IReadOnlyList<ActividadDiaItem> ActividadSemanal
    {
        get => _actividadSemanal;
        private set => SetProperty(ref _actividadSemanal, value);
    }

    public bool FiltroTodasActivo => _filtroActivo == "Todas";
    public bool FiltroPendientesActivo => _filtroActivo == "Pendientes";
    public bool FiltroEnCursoActivo => _filtroActivo == "EnCurso";
    public bool FiltroCompletadasActivo => _filtroActivo == "Completadas";

    public bool TieneFiltroProyecto => _proyectoFiltroId.HasValue;

    public string ProyectoFiltroNombre
    {
        get => _proyectoFiltroNombre;
        private set => SetProperty(ref _proyectoFiltroNombre, value);
    }

    public string ModoVista
    {
        get => _modoVista;
        private set
        {
            if (!SetProperty(ref _modoVista, value))
                return;

            OnPropertyChanged(nameof(EsVistaLista));
            OnPropertyChanged(nameof(EsVistaKanban));
            OnPropertyChanged(nameof(EsVistaCalendario));
            OnPropertyChanged(nameof(ModoListaActivo));
            OnPropertyChanged(nameof(ModoKanbanActivo));
            OnPropertyChanged(nameof(ModoCalendarioActivo));
        }
    }

    public bool EsVistaLista => ModoVista == "Lista";
    public bool EsVistaKanban => ModoVista == "Kanban";
    public bool EsVistaCalendario => ModoVista == "Calendario";

    public bool ModoListaActivo => ModoVista == "Lista";
    public bool ModoKanbanActivo => ModoVista == "Kanban";
    public bool ModoCalendarioActivo => ModoVista == "Calendario";

    public ObservableCollection<KanbanColumnItem> ColumnasKanban => _columnasKanban;
    public ObservableCollection<CalendarioDiaItem> DiasCalendario => _diasCalendario;

    public string MesAnioTexto
    {
        get => _mesAnioTexto;
        private set => SetProperty(ref _mesAnioTexto, value);
    }

    public IEnumerable<TareaListItem> Tareas
    {
        get => _tareasFiltradas;
        private set
        {
            SetProperty(ref _tareasFiltradas, value);
            NotificarEstadoLista();
            OnPropertyChanged(nameof(InfoPaginacion));
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

    public TareaListItem? TareaSeleccionada
    {
        get => _tareaSeleccionada;
        set
        {
            SetProperty(ref _tareaSeleccionada, value);
            EditarTareaCommand.NotifyCanExecuteChanged();
            EliminarTareaCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand CargarTareasCommand { get; }
    public IRelayCommand CrearTareaCommand { get; }
    public IRelayCommand<TareaListItem> EditarTareaCommand { get; }
    public IAsyncRelayCommand<TareaListItem> EliminarTareaCommand { get; }
    public IAsyncRelayCommand ExportarCommand { get; }
    public IRelayCommand PaginaAnteriorCommand { get; }
    public IRelayCommand PaginaSiguienteCommand { get; }
    public IRelayCommand FiltrarTodasCommand { get; }
    public IRelayCommand FiltrarPendientesCommand { get; }
    public IRelayCommand FiltrarEnCursoCommand { get; }
    public IRelayCommand FiltrarCompletadasCommand { get; }
    public IRelayCommand VerListaCommand { get; }
    public IRelayCommand VerKanbanCommand { get; }
    public IRelayCommand VerCalendarioCommand { get; }
    public IAsyncRelayCommand<KanbanMoveRequest> MoverKanbanCommand { get; }
    public IRelayCommand MesAnteriorCommand { get; }
    public IRelayCommand MesSiguienteCommand { get; }

    public event EventHandler<Tarea>? TareaCreada;
    public event EventHandler<string>? ExportacionCompletada;
    public event EventHandler<Tarea>? TareaEditada;

    public bool ToolbarExportVisible => CanExport;
    public bool ToolbarCreateVisible => CanManage;
    public string ToolbarCreateLabel => "Nueva tarea";
    public IAsyncRelayCommand? ToolbarExportCommand => ExportarCommand;
    public IRelayCommand? ToolbarCreateCommand => CrearTareaCommand;

    private async Task CargarTareasAsync()
    {
        IsLoading = true;
        NotificarEstadoLista();
        try
        {
            _tareas = (await _tareaService.GetAllAsync()).ToList();

            if (_proyectoFiltroId.HasValue)
            {
                var proyecto = _tareas.FirstOrDefault(t => t.ProyectoId == _proyectoFiltroId)?.Proyecto;
                ProyectoFiltroNombre = proyecto?.Nombre ?? $"Proyecto #{_proyectoFiltroId}";
                OnPropertyChanged(nameof(TieneFiltroProyecto));
            }

            ActualizarEstadisticas();
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

    private void ActualizarEstadisticas()
    {
        var lista = _tareas.ToList();
        TotalTareas = lista.Count;
        Pendientes = lista.Count(t => t.Estado == EstadoTarea.Pendiente);
        EnCurso = lista.Count(t => t.Estado is EstadoTarea.EnProgreso or EstadoTarea.EnRevision);
        Completadas = lista.Count(t => t.Estado == EstadoTarea.Completada);

        var hoy = DateTime.Today;
        var inicioSemana = hoy.AddDays(-6);
        var inicioSemanaAnterior = inicioSemana.AddDays(-7);

        var completadasSemana = lista.Count(t =>
            t.Estado == EstadoTarea.Completada &&
            (t.FechaFin?.Date ?? t.FechaCreacion.Date) >= inicioSemana);

        var totalActivas = lista.Count(t => t.Estado != EstadoTarea.Cancelada);
        RendimientoPorcentaje = totalActivas == 0
            ? 0
            : (int)Math.Round(100.0 * Completadas / totalActivas);

        var completadasSemanaAnterior = lista.Count(t =>
            t.Estado == EstadoTarea.Completada &&
            (t.FechaFin?.Date ?? t.FechaCreacion.Date) >= inicioSemanaAnterior &&
            (t.FechaFin?.Date ?? t.FechaCreacion.Date) < inicioSemana);

        var delta = completadasSemana - completadasSemanaAnterior;
        RendimientoVariacion = delta switch
        {
            > 0 => $"+{delta} tareas completadas vs. la semana anterior",
            < 0 => $"{delta} tareas completadas vs. la semana anterior",
            _ => "Sin cambios respecto a la semana anterior"
        };

        var dias = Enumerable.Range(0, 7)
            .Select(i => inicioSemana.AddDays(i))
            .ToList();

        var valores = dias.Select(dia =>
            lista.Count(t =>
                t.FechaCreacion.Date == dia.Date ||
                (t.FechaFin?.Date == dia.Date && t.Estado == EstadoTarea.Completada)))
            .ToList();

        var max = valores.DefaultIfEmpty(0).Max();
        if (max == 0) max = 1;

        ActividadSemanal = dias.Select((dia, i) => new ActividadDiaItem
        {
            Etiqueta = dia.ToString("ddd", System.Globalization.CultureInfo.GetCultureInfo("es-ES")),
            Valor = valores[i],
            AlturaPorcentaje = Math.Max(8, valores[i] * 100.0 / max),
            EsDiaActual = dia.Date == hoy
        }).ToList();
    }

    private void AplicarFiltroEstado(string filtro)
    {
        _filtroActivo = filtro;
        OnPropertyChanged(nameof(FiltroTodasActivo));
        OnPropertyChanged(nameof(FiltroPendientesActivo));
        OnPropertyChanged(nameof(FiltroEnCursoActivo));
        OnPropertyChanged(nameof(FiltroCompletadasActivo));
        FiltrarTareas();
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

    private void EditarTarea(TareaListItem? item)
    {
        if (item != null)
            TareaEditada?.Invoke(this, item.Tarea);
    }

    private bool CanEditarTarea(TareaListItem? item) => item != null;

    private async Task EliminarTareaAsync(TareaListItem? item)
    {
        if (item == null) return;

        var tarea = item.Tarea;
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

    private bool CanEliminarTarea(TareaListItem? item) => item != null;

    private void FiltrarTareas()
    {
        var consulta = _tareas.AsEnumerable();

        if (_proyectoFiltroId.HasValue)
            consulta = consulta.Where(t => t.ProyectoId == _proyectoFiltroId);

        consulta = _filtroActivo switch
        {
            "Pendientes" => consulta.Where(t => t.Estado == EstadoTarea.Pendiente),
            "EnCurso" => consulta.Where(t => t.Estado is EstadoTarea.EnProgreso or EstadoTarea.EnRevision),
            "Completadas" => consulta.Where(t => t.Estado == EstadoTarea.Completada),
            _ => consulta
        };

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            consulta = SearchHelper.FilterByText(
                consulta,
                TextoBusqueda,
                t => t.Titulo,
                t => t.Descripcion,
                t => t.Proyecto?.Nombre ?? string.Empty,
                t => t.Asignado?.Nombre ?? string.Empty,
                t => t.Asignado?.Apellidos ?? string.Empty);
        }

        _resultadoFiltrado = consulta.ToList();
        _paginacion.Reiniciar();
        AplicarPaginacion();
        ConstruirKanban();
        ConstruirCalendario();
    }

    private void AplicarPaginacion()
    {
        _paginacion.TamañoPagina = Math.Max(1, _resultadoFiltrado.Count);
        var pagina = _paginacion.Aplicar(_resultadoFiltrado).Select(MapToListItem).ToList();
        Tareas = pagina;
        OnPropertyChanged(nameof(InfoPaginacion));
        PaginaAnteriorCommand.NotifyCanExecuteChanged();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
    }

    private void CambiarModoVista(string modo) => ModoVista = modo;

    private void ConstruirKanban()
    {
        var items = _resultadoFiltrado.Select(MapToListItem).ToList();
        _columnasKanban.Clear();

        foreach (var def in DefinicionesKanban)
        {
            var columna = new KanbanColumnItem
            {
                Titulo = def.Titulo,
                Estado = def.Estado,
                EstadoSiguiente = def.Siguiente,
                TextoMoverSiguiente = def.TextoMover
            };

            foreach (var item in items.Where(i => i.Tarea.Estado == def.Estado))
                columna.Tarjetas.Add(item);

            _columnasKanban.Add(columna);
        }
    }

    private void ConstruirCalendario()
    {
        var inicioMes = new DateTime(_mesCalendario.Year, _mesCalendario.Month, 1);
        var inicioGrilla = inicioMes.AddDays(-(((int)inicioMes.DayOfWeek + 6) % 7));
        var items = _resultadoFiltrado.Select(MapToListItem).ToList();
        var hoy = DateTime.Today;

        _diasCalendario.Clear();
        for (var i = 0; i < 42; i++)
        {
            var fecha = inicioGrilla.AddDays(i);
            var tareasDia = items
                .Where(item => ObtenerFechaCalendario(item.Tarea)?.Date == fecha.Date)
                .ToList();

            _diasCalendario.Add(new CalendarioDiaItem
            {
                Numero = fecha.Day,
                EsMesActual = fecha.Month == _mesCalendario.Month,
                EsHoy = fecha.Date == hoy,
                Tareas = tareasDia
            });
        }

        ActualizarMesAnioTexto();
    }

    private static DateTime? ObtenerFechaCalendario(Tarea tarea) =>
        tarea.FechaVencimiento?.Date ?? tarea.FechaInicio?.Date ?? tarea.FechaFin?.Date;

    private void ActualizarMesAnioTexto()
    {
        var cultura = CultureInfo.GetCultureInfo("es-ES");
        var texto = _mesCalendario.ToString("MMMM yyyy", cultura);
        MesAnioTexto = char.ToUpper(texto[0], cultura) + texto[1..];
    }

    private void IrMesAnterior()
    {
        _mesCalendario = _mesCalendario.AddMonths(-1);
        ConstruirCalendario();
    }

    private void IrMesSiguiente()
    {
        _mesCalendario = _mesCalendario.AddMonths(1);
        ConstruirCalendario();
    }

    private bool CanMoverKanban(KanbanMoveRequest? request) =>
        CanManage && request?.Item != null;

    private async Task MoverKanbanAsync(KanbanMoveRequest? request)
    {
        if (request?.Item == null)
            return;

        IsLoading = true;
        try
        {
            await _tareaExtension.CambiarEstadoAsync(request.Item.Tarea.Id, request.NuevoEstado);
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

    private static TareaListItem MapToListItem(Tarea tarea)
    {
        var (prioridadTexto, prioridadFondo, prioridadColor) = tarea.Prioridad switch
        {
            PrioridadTarea.Critica or PrioridadTarea.Alta => ("Alta", "#FEE2E2", "#B91C1C"),
            PrioridadTarea.Media => ("Media", "#FEF3C7", "#B45309"),
            _ => ("Baja", "#F1F5F9", "#475569")
        };

        var (estadoTexto, estadoFondo, estadoColor) = tarea.Estado switch
        {
            EstadoTarea.EnProgreso => ("En progreso", "#DBEAFE", "#1D4ED8"),
            EstadoTarea.EnRevision => ("En revisión", "#E0E7FF", "#4338CA"),
            EstadoTarea.Completada => ("Completada", "#DCFCE7", "#15803D"),
            EstadoTarea.Cancelada => ("Cancelada", "#FFEBEB", "#BA1A1A"),
            _ => ("Pendiente", "#FEF3C7", "#B45309")
        };

        var asignado = tarea.Asignado;
        var avatarIndex = asignado?.AvatarIndex ?? 0;
        if (avatarIndex == 0 && asignado != null && asignado.Id > 0)
            avatarIndex = (asignado.Id - 1) % AvatarCatalog.Count;

        return new TareaListItem
        {
            Tarea = tarea,
            PrioridadTexto = prioridadTexto,
            PrioridadFondo = prioridadFondo,
            PrioridadTextoColor = prioridadColor,
            EstadoTexto = estadoTexto,
            EstadoFondo = estadoFondo,
            EstadoTextoColor = estadoColor,
            NombreProyecto = string.IsNullOrWhiteSpace(tarea.Proyecto?.Nombre)
                ? "Sin proyecto"
                : $"Proyecto: {tarea.Proyecto!.Nombre}",
            AsignadoNombre = asignado == null
                ? "Sin asignar"
                : $"{asignado.Nombre} {asignado.Apellidos}".Trim(),
            AsignadoIniciales = GetIniciales(asignado?.Nombre, asignado?.Apellidos),
            AvatarIndex = Math.Clamp(avatarIndex, 0, AvatarCatalog.Count - 1),
            FechaLimiteTexto = tarea.FechaVencimiento?.ToString("dd MMM, yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")) ?? "—",
            TituloTachado = tarea.Estado == EstadoTarea.Completada
        };
    }

    private static string GetIniciales(string? nombre, string? apellidos)
    {
        var n = string.IsNullOrWhiteSpace(nombre) ? "?" : nombre.Trim()[0].ToString();
        var a = string.IsNullOrWhiteSpace(apellidos) ? string.Empty : apellidos.Trim()[0].ToString();
        return $"{n}{a}".ToUpperInvariant();
    }
}
