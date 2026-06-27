using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel del formulario de tarea (alta/edición).</summary>
public class TareaFormViewModel : ViewModelBase
{
    private readonly ITareaService _tareaService;
    private readonly IProyectoService _proyectoService;
    private readonly IEmpleadoService _empleadoService;
    private readonly ITareaExtensionService _extensionService;
    private readonly IAttachmentService _attachmentService;
    private readonly IActivityLogService _activityLogService;
    private Tarea _tarea;
    private bool _esNuevo;
    private IEnumerable<Proyecto> _proyectos = new List<Proyecto>();
    private IEnumerable<Empleado> _empleados = new List<Empleado>();
    private IEnumerable<Tarea> _tareasDisponibles = new List<Tarea>();
    private ObservableCollection<ComentarioTareaItem> _comentarios = new();
    private ObservableCollection<SubTarea> _subtareas = new();
    private ObservableCollection<int> _dependenciasIds = new();
    private ObservableCollection<RegistroTiempo> _registrosTiempo = new();
    private ObservableCollection<TareaAdjuntoInfo> _adjuntos = new();
    private ObservableCollection<ActivityLogEntry> _historialActividad = new();
    private string _nuevoComentario = string.Empty;
    private string _nuevaSubtarea = string.Empty;
    private int? _dependenciaSeleccionadaId;
    private int _nuevoMinutos;
    private string _nuevoNotaTiempo = string.Empty;
    private int _minutosTotales;
    private string _seccionActiva = "General";

    public TareaFormViewModel(
        ITareaService tareaService,
        IProyectoService proyectoService,
        IEmpleadoService empleadoService,
        ITareaExtensionService extensionService,
        IAttachmentService attachmentService,
        IActivityLogService activityLogService,
        Tarea? tarea = null)
    {
        _tareaService = tareaService;
        _proyectoService = proyectoService;
        _empleadoService = empleadoService;
        _extensionService = extensionService;
        _attachmentService = attachmentService;
        _activityLogService = activityLogService;
        _tarea = tarea ?? new Tarea
        {
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Media,
            FechaCreacion = DateTime.Now
        };
        _esNuevo = tarea == null;

        GuardarCommand = new AsyncRelayCommand(GuardarAsync, CanGuardar);
        CancelarCommand = new RelayCommand(Cancelar);
        CargarDatosCommand = new AsyncRelayCommand(CargarDatosAsync);
        AgregarComentarioCommand = new AsyncRelayCommand(AgregarComentarioAsync, CanAgregarComentario);
        AgregarSubtareaCommand = new AsyncRelayCommand(AgregarSubtareaAsync, CanAgregarSubtarea);
        ToggleSubtareaCommand = new AsyncRelayCommand<SubTarea>(ToggleSubtareaAsync);
        AgregarDependenciaCommand = new AsyncRelayCommand(AgregarDependenciaAsync, CanAgregarDependencia);
        RegistrarTiempoCommand = new AsyncRelayCommand(RegistrarTiempoAsync, CanRegistrarTiempo);
        AgregarAdjuntoCommand = new RelayCommand(SolicitarAgregarAdjunto);
        AbrirAdjuntoCommand = new RelayCommand<TareaAdjuntoInfo>(AbrirAdjunto);
        EliminarAdjuntoCommand = new AsyncRelayCommand<TareaAdjuntoInfo>(EliminarAdjuntoAsync);
        SeleccionarSeccionCommand = new RelayCommand<string>(s => SeccionActiva = s ?? "General");

        CargarDatosCommand.ExecuteAsync(null);
    }

    public string Titulo => _esNuevo ? "Nueva Tarea" : "Editar Tarea";

    public string Subtitulo => _esNuevo
        ? "Registra una nueva tarea para el equipo"
        : "Actualiza el estado y los detalles de la tarea";

    public bool EsNuevo => _esNuevo;
    public bool MuestraExtensiones => !_esNuevo;
    public bool MuestraComentarios => !_esNuevo;

    public string SeccionActiva
    {
        get => _seccionActiva;
        set
        {
            if (!SetProperty(ref _seccionActiva, value))
                return;

            OnPropertyChanged(nameof(EsSeccionGeneral));
            OnPropertyChanged(nameof(EsSeccionSubtareas));
            OnPropertyChanged(nameof(EsSeccionDependencias));
            OnPropertyChanged(nameof(EsSeccionTiempo));
            OnPropertyChanged(nameof(EsSeccionAdjuntos));
            OnPropertyChanged(nameof(EsSeccionHistorial));
            OnPropertyChanged(nameof(EsSeccionComentarios));
        }
    }

    public bool EsSeccionGeneral => SeccionActiva == "General";
    public bool EsSeccionSubtareas => SeccionActiva == "Subtareas";
    public bool EsSeccionDependencias => SeccionActiva == "Dependencias";
    public bool EsSeccionTiempo => SeccionActiva == "Tiempo";
    public bool EsSeccionAdjuntos => SeccionActiva == "Adjuntos";
    public bool EsSeccionHistorial => SeccionActiva == "Historial";
    public bool EsSeccionComentarios => SeccionActiva == "Comentarios";

    public string TituloTarea
    {
        get => _tarea.Titulo;
        set
        {
            _tarea.Titulo = value;
            OnPropertyChanged();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Descripcion
    {
        get => _tarea.Descripcion;
        set
        {
            _tarea.Descripcion = value;
            OnPropertyChanged();
        }
    }

    public PrioridadTarea Prioridad
    {
        get => _tarea.Prioridad;
        set
        {
            _tarea.Prioridad = value;
            OnPropertyChanged();
        }
    }

    public EstadoTarea Estado
    {
        get => _tarea.Estado;
        set
        {
            _tarea.Estado = value;
            OnPropertyChanged();
        }
    }

    public DateTime? FechaVencimiento
    {
        get => _tarea.FechaVencimiento;
        set
        {
            _tarea.FechaVencimiento = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<Proyecto> Proyectos
    {
        get => _proyectos;
        set => SetProperty(ref _proyectos, value);
    }

    public IEnumerable<Empleado> Empleados
    {
        get => _empleados;
        set => SetProperty(ref _empleados, value);
    }

    public IEnumerable<Tarea> TareasDisponibles
    {
        get => _tareasDisponibles;
        set => SetProperty(ref _tareasDisponibles, value);
    }

    public int? ProyectoId
    {
        get => _tarea.ProyectoId;
        set
        {
            _tarea.ProyectoId = value;
            OnPropertyChanged();
        }
    }

    public int? AsignadoId
    {
        get => _tarea.AsignadoId;
        set
        {
            _tarea.AsignadoId = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<PrioridadTarea> PrioridadesTarea { get; } =
        Enum.GetValues(typeof(PrioridadTarea)).Cast<PrioridadTarea>();

    public IEnumerable<EstadoTarea> EstadosTarea { get; } =
        Enum.GetValues(typeof(EstadoTarea)).Cast<EstadoTarea>();

    public ObservableCollection<ComentarioTareaItem> Comentarios
    {
        get => _comentarios;
        set => SetProperty(ref _comentarios, value);
    }

    public ObservableCollection<SubTarea> Subtareas
    {
        get => _subtareas;
        set => SetProperty(ref _subtareas, value);
    }

    public ObservableCollection<int> DependenciasIds
    {
        get => _dependenciasIds;
        set => SetProperty(ref _dependenciasIds, value);
    }

    public ObservableCollection<RegistroTiempo> RegistrosTiempo
    {
        get => _registrosTiempo;
        set => SetProperty(ref _registrosTiempo, value);
    }

    public ObservableCollection<TareaAdjuntoInfo> Adjuntos
    {
        get => _adjuntos;
        set => SetProperty(ref _adjuntos, value);
    }

    public ObservableCollection<ActivityLogEntry> HistorialActividad
    {
        get => _historialActividad;
        set => SetProperty(ref _historialActividad, value);
    }

    public string NuevoComentario
    {
        get => _nuevoComentario;
        set
        {
            SetProperty(ref _nuevoComentario, value);
            AgregarComentarioCommand.NotifyCanExecuteChanged();
        }
    }

    public string NuevaSubtarea
    {
        get => _nuevaSubtarea;
        set
        {
            SetProperty(ref _nuevaSubtarea, value);
            AgregarSubtareaCommand.NotifyCanExecuteChanged();
        }
    }

    public int? DependenciaSeleccionadaId
    {
        get => _dependenciaSeleccionadaId;
        set
        {
            SetProperty(ref _dependenciaSeleccionadaId, value);
            AgregarDependenciaCommand.NotifyCanExecuteChanged();
        }
    }

    public int NuevoMinutos
    {
        get => _nuevoMinutos;
        set
        {
            SetProperty(ref _nuevoMinutos, value);
            RegistrarTiempoCommand.NotifyCanExecuteChanged();
        }
    }

    public string NuevoNotaTiempo
    {
        get => _nuevoNotaTiempo;
        set => SetProperty(ref _nuevoNotaTiempo, value);
    }

    public int MinutosTotales
    {
        get => _minutosTotales;
        set => SetProperty(ref _minutosTotales, value);
    }

    public string DependenciasTexto =>
        DependenciasIds.Count == 0
            ? "Sin dependencias"
            : string.Join(", ", DependenciasIds.Select(id => $"#{id}"));

    public IAsyncRelayCommand GuardarCommand { get; }
    public IRelayCommand CancelarCommand { get; }
    public IAsyncRelayCommand CargarDatosCommand { get; }
    public IAsyncRelayCommand AgregarComentarioCommand { get; }
    public IAsyncRelayCommand AgregarSubtareaCommand { get; }
    public IAsyncRelayCommand<SubTarea> ToggleSubtareaCommand { get; }
    public IAsyncRelayCommand AgregarDependenciaCommand { get; }
    public IAsyncRelayCommand RegistrarTiempoCommand { get; }
    public IRelayCommand AgregarAdjuntoCommand { get; }
    public IRelayCommand<TareaAdjuntoInfo> AbrirAdjuntoCommand { get; }
    public IAsyncRelayCommand<TareaAdjuntoInfo> EliminarAdjuntoCommand { get; }
    public IRelayCommand<string> SeleccionarSeccionCommand { get; }

    public event EventHandler? Guardado;
    public event EventHandler? Cancelado;
    public event EventHandler? SolicitarAdjunto;
    public event EventHandler<string>? AbrirArchivoSolicitado;

    private async Task CargarDatosAsync()
    {
        try
        {
            Proyectos = await _proyectoService.GetAllAsync();
            Empleados = await _empleadoService.GetActivosAsync();

            if (!_esNuevo && _tarea.Id > 0)
            {
                var todasTareas = (await _tareaService.GetAllAsync()).ToList();
                TareasDisponibles = todasTareas.Where(t => t.Id != _tarea.Id).ToList();

                var tareaCompleta = await _tareaService.GetByIdAsync(_tarea.Id);
                if (tareaCompleta != null)
                {
                    Comentarios = new ObservableCollection<ComentarioTareaItem>(
                        tareaCompleta.Comentarios
                            .OrderByDescending(c => c.FechaCreacion)
                            .Select(c => new ComentarioTareaItem
                            {
                                Contenido = c.Contenido,
                                Autor = c.Empleado != null
                                    ? $"{c.Empleado.Nombre} {c.Empleado.Apellidos}"
                                    : SessionService.GetUserName(),
                                FechaCreacion = c.FechaCreacion
                            }));
                }

                var subtareas = await _extensionService.GetSubtareasAsync(_tarea.Id);
                Subtareas = new ObservableCollection<SubTarea>(subtareas);

                var deps = await _extensionService.GetDependenciasAsync(_tarea.Id);
                DependenciasIds = new ObservableCollection<int>(deps);
                OnPropertyChanged(nameof(DependenciasTexto));

                var registros = await _extensionService.GetRegistrosTiempoAsync(_tarea.Id);
                RegistrosTiempo = new ObservableCollection<RegistroTiempo>(registros);
                MinutosTotales = await _extensionService.GetMinutosTotalesAsync(_tarea.Id);

                var adjuntos = await _attachmentService.GetByTareaAsync(_tarea.Id);
                Adjuntos = new ObservableCollection<TareaAdjuntoInfo>(adjuntos);

                var historial = await _activityLogService.GetPorEntidadAsync("Tarea", _tarea.Id);
                HistorialActividad = new ObservableCollection<ActivityLogEntry>(historial);
            }
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
    }

    private bool CanAgregarComentario() =>
        !_esNuevo && !string.IsNullOrWhiteSpace(NuevoComentario);

    private bool CanAgregarSubtarea() =>
        !_esNuevo && !string.IsNullOrWhiteSpace(NuevaSubtarea);

    private bool CanAgregarDependencia() =>
        !_esNuevo && DependenciaSeleccionadaId.HasValue && DependenciaSeleccionadaId != _tarea.Id;

    private bool CanRegistrarTiempo() =>
        !_esNuevo && NuevoMinutos > 0;

    private async Task AgregarComentarioAsync()
    {
        if (_esNuevo || _tarea.Id <= 0) return;

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var empleadoId = await ObtenerEmpleadoIdActualAsync();
            var comentario = new ComentarioTarea
            {
                Contenido = NuevoComentario.Trim(),
                EmpleadoId = empleadoId
            };

            await _tareaService.AgregarComentarioAsync(_tarea.Id, comentario);
            NuevoComentario = string.Empty;

            Comentarios.Insert(0, new ComentarioTareaItem
            {
                Contenido = comentario.Contenido,
                Autor = SessionService.GetUserName(),
                FechaCreacion = DateTime.Now
            });
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

    private async Task AgregarSubtareaAsync()
    {
        if (_esNuevo || _tarea.Id <= 0) return;

        IsLoading = true;
        try
        {
            var st = await _extensionService.AgregarSubtareaAsync(_tarea.Id, NuevaSubtarea.Trim());
            Subtareas.Add(st);
            NuevaSubtarea = string.Empty;
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

    private async Task ToggleSubtareaAsync(SubTarea? subtarea)
    {
        if (subtarea == null) return;

        subtarea.Completada = !subtarea.Completada;
        try
        {
            await _extensionService.ActualizarSubtareaAsync(subtarea);
            var idx = Subtareas.IndexOf(subtarea);
            if (idx >= 0)
            {
                Subtareas.RemoveAt(idx);
                Subtareas.Insert(idx, subtarea);
            }
        }
        catch (Exception ex)
        {
            subtarea.Completada = !subtarea.Completada;
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
    }

    private async Task AgregarDependenciaAsync()
    {
        if (_esNuevo || _tarea.Id <= 0 || !DependenciaSeleccionadaId.HasValue) return;

        IsLoading = true;
        try
        {
            await _extensionService.AgregarDependenciaAsync(_tarea.Id, DependenciaSeleccionadaId.Value);
            if (!DependenciasIds.Contains(DependenciaSeleccionadaId.Value))
                DependenciasIds.Add(DependenciaSeleccionadaId.Value);
            OnPropertyChanged(nameof(DependenciasTexto));
            DependenciaSeleccionadaId = null;
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

    private async Task RegistrarTiempoAsync()
    {
        if (_esNuevo || _tarea.Id <= 0) return;

        IsLoading = true;
        try
        {
            var empleadoId = await ObtenerEmpleadoIdActualAsync();
            var reg = await _extensionService.RegistrarTiempoAsync(
                _tarea.Id, NuevoMinutos, NuevoNotaTiempo.Trim(), empleadoId);
            RegistrosTiempo.Insert(0, reg);
            MinutosTotales += NuevoMinutos;
            NuevoMinutos = 0;
            NuevoNotaTiempo = string.Empty;
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

    private void SolicitarAgregarAdjunto() => SolicitarAdjunto?.Invoke(this, EventArgs.Empty);

    public async Task AgregarAdjuntoDesdeRutaAsync(string rutaOrigen)
    {
        if (_esNuevo || _tarea.Id <= 0) return;

        IsLoading = true;
        try
        {
            var empleadoId = await ObtenerEmpleadoIdActualAsync();
            var info = await _attachmentService.AgregarAsync(_tarea.Id, rutaOrigen, empleadoId);
            Adjuntos.Insert(0, info);
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

    private void AbrirAdjunto(TareaAdjuntoInfo? adjunto)
    {
        if (adjunto == null) return;
        var ruta = _attachmentService.ObtenerRutaCompleta(adjunto.RutaRelativa);
        AbrirArchivoSolicitado?.Invoke(this, ruta);
    }

    private async Task EliminarAdjuntoAsync(TareaAdjuntoInfo? adjunto)
    {
        if (adjunto == null) return;

        if (!SolicitarConfirmacion($"¿Eliminar el adjunto «{adjunto.NombreArchivo}»?", "Eliminar adjunto"))
            return;

        IsLoading = true;
        try
        {
            await _attachmentService.EliminarAsync(adjunto.Id);
            Adjuntos.Remove(adjunto);
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

    private async Task<int?> ObtenerEmpleadoIdActualAsync()
    {
        var usuario = SessionService.CurrentUser;
        if (usuario == null) return null;

        var empleados = await _empleadoService.GetAllAsync();
        var empleado = empleados.FirstOrDefault(e => e.UsuarioId == usuario.Id)
            ?? empleados.FirstOrDefault(e =>
                e.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase));

        return empleado?.Id;
    }

    private bool CanGuardar() => !string.IsNullOrWhiteSpace(TituloTarea);

    private async Task GuardarAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            if (_esNuevo)
            {
                _tarea.CreadorId = await ObtenerEmpleadoIdActualAsync();
                await _tareaService.CreateAsync(_tarea);
            }
            else
            {
                await _tareaService.UpdateAsync(_tarea);
            }

            Guardado?.Invoke(this, EventArgs.Empty);
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

    private void Cancelar() => Cancelado?.Invoke(this, EventArgs.Empty);
}
