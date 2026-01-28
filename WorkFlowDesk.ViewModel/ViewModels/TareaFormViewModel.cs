using System.Linq;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class TareaFormViewModel : ViewModelBase
{
    private readonly ITareaService _tareaService;
    private readonly IProyectoService _proyectoService;
    private readonly IEmpleadoService _empleadoService;
    private Tarea _tarea;
    private bool _esNuevo;
    private IEnumerable<Proyecto> _proyectos = new List<Proyecto>();
    private IEnumerable<Empleado> _empleados = new List<Empleado>();

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
        
        CargarDatosCommand.ExecuteAsync(null);
    }

    public string Titulo => _esNuevo ? "Nueva Tarea" : "Editar Tarea";

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

    public IAsyncRelayCommand GuardarCommand { get; }
    public IRelayCommand CancelarCommand { get; }
    public IAsyncRelayCommand CargarDatosCommand { get; }

    public event EventHandler? Guardado;
    public event EventHandler? Cancelado;

    private async Task CargarDatosAsync()
    {
        try
        {
            Proyectos = await _proyectoService.GetAllAsync();
            Empleados = await _empleadoService.GetActivosAsync();
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
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
