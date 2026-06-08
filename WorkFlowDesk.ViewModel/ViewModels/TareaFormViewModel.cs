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
    private Tarea _tarea;
    private bool _esNuevo;
    private IEnumerable<Proyecto> _proyectos = new List<Proyecto>();
    private IEnumerable<Empleado> _empleados = new List<Empleado>();
    private ObservableCollection<ComentarioTareaItem> _comentarios = new();
    private string _nuevoComentario = string.Empty;

    public TareaFormViewModel(
        ITareaService tareaService,
        IProyectoService proyectoService,
        IEmpleadoService empleadoService,
        Tarea? tarea = null)
    {
        _tareaService = tareaService;
        _proyectoService = proyectoService;
        _empleadoService = empleadoService;
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
        
        CargarDatosCommand.ExecuteAsync(null);
    }

    public string Titulo => _esNuevo ? "Nueva Tarea" : "Editar Tarea";
    public bool EsNuevo => _esNuevo;
    public bool MuestraComentarios => !_esNuevo;

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

    public string NuevoComentario
    {
        get => _nuevoComentario;
        set
        {
            SetProperty(ref _nuevoComentario, value);
            AgregarComentarioCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand GuardarCommand { get; }
    public IRelayCommand CancelarCommand { get; }
    public IAsyncRelayCommand CargarDatosCommand { get; }
    public IAsyncRelayCommand AgregarComentarioCommand { get; }

    public event EventHandler? Guardado;
    public event EventHandler? Cancelado;

    private async Task CargarDatosAsync()
    {
        try
        {
            Proyectos = await _proyectoService.GetAllAsync();
            Empleados = await _empleadoService.GetActivosAsync();

            if (!_esNuevo && _tarea.Id > 0)
            {
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

    private bool CanGuardar()
    {
        return !string.IsNullOrWhiteSpace(TituloTarea);
    }

    private async Task GuardarAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            if (_esNuevo)
            {
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

    private void Cancelar()
    {
        Cancelado?.Invoke(this, EventArgs.Empty);
    }
}
