using System.Linq;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class ProyectoFormViewModel : ViewModelBase
{
    private readonly IProyectoService _proyectoService;
    private readonly IClienteService _clienteService;
    private readonly IEmpleadoService _empleadoService;
    private Proyecto _proyecto;
    private bool _esNuevo;
    private IEnumerable<Cliente> _clientes = new List<Cliente>();
    private IEnumerable<Empleado> _empleados = new List<Empleado>();

    public ProyectoFormViewModel(
        IProyectoService proyectoService,
        IClienteService clienteService,
        IEmpleadoService empleadoService,
        Proyecto? proyecto = null)
    {
        _proyectoService = proyectoService;
        _clienteService = clienteService;
        _empleadoService = empleadoService;
        _proyecto = proyecto ?? new Proyecto
        {
            Estado = EstadoProyecto.Planificacion,
            FechaInicio = DateTime.Now
        };
        _esNuevo = proyecto == null;

        GuardarCommand = new AsyncRelayCommand(GuardarAsync, CanGuardar);
        CancelarCommand = new RelayCommand(Cancelar);
        CargarDatosCommand = new AsyncRelayCommand(CargarDatosAsync);
        
        CargarDatosCommand.ExecuteAsync(null);
    }

    public string Titulo => _esNuevo ? "Nuevo Proyecto" : "Editar Proyecto";

    public string Nombre
    {
        get => _proyecto.Nombre;
        set
        {
            _proyecto.Nombre = value;
            OnPropertyChanged();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Descripcion
    {
        get => _proyecto.Descripcion;
        set
        {
            _proyecto.Descripcion = value;
            OnPropertyChanged();
        }
    }

    public EstadoProyecto Estado
    {
        get => _proyecto.Estado;
        set
        {
            _proyecto.Estado = value;
            OnPropertyChanged();
        }
    }

    public DateTime FechaInicio
    {
        get => _proyecto.FechaInicio;
        set
        {
            _proyecto.FechaInicio = value;
            OnPropertyChanged();
        }
    }

    public DateTime? FechaFin
    {
        get => _proyecto.FechaFin;
        set
        {
            _proyecto.FechaFin = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<Cliente> Clientes
    {
        get => _clientes;
        set => SetProperty(ref _clientes, value);
    }

    public IEnumerable<Empleado> Empleados
    {
        get => _empleados;
        set => SetProperty(ref _empleados, value);
    }

    public int? ClienteId
    {
        get => _proyecto.ClienteId;
        set
        {
            _proyecto.ClienteId = value;
            OnPropertyChanged();
        }
    }

    public int? ResponsableId
    {
        get => _proyecto.ResponsableId;
        set
        {
            _proyecto.ResponsableId = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<EstadoProyecto> EstadosProyecto { get; } =
        Enum.GetValues(typeof(EstadoProyecto)).Cast<EstadoProyecto>();

    public IAsyncRelayCommand GuardarCommand { get; }
    public IRelayCommand CancelarCommand { get; }
    public IAsyncRelayCommand CargarDatosCommand { get; }

    public event EventHandler? Guardado;
    public event EventHandler? Cancelado;

    private async Task CargarDatosAsync()
    {
        try
        {
            Clientes = await _clienteService.GetActivosAsync();
            Empleados = await _empleadoService.GetActivosAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar datos: {ex.Message}";
        }
    }

    private bool CanGuardar()
    {
        return !string.IsNullOrWhiteSpace(Nombre);
    }

    private async Task GuardarAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        if (FechaFin.HasValue && !DateRangeValidator.IsValidDateRange(FechaInicio, FechaFin))
        {
            ErrorMessage = "La fecha de fin debe ser posterior o igual a la fecha de inicio.";
            IsLoading = false;
            return;
        }

        try
        {
            if (_esNuevo)
            {
                await _proyectoService.CreateAsync(_proyecto);
            }
            else
            {
                await _proyectoService.UpdateAsync(_proyecto);
            }

            Guardado?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al guardar proyecto: {ex.Message}";
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
