using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class EmpleadosViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;
    private IEnumerable<Empleado> _empleados = new List<Empleado>();
    private IEnumerable<Empleado> _empleadosFiltrados = new List<Empleado>();
    private Empleado? _empleadoSeleccionado;
    private string _textoBusqueda = string.Empty;

    public EmpleadosViewModel(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
        CargarEmpleadosCommand = new AsyncRelayCommand(CargarEmpleadosAsync);
        CrearEmpleadoCommand = new RelayCommand(CrearEmpleado);
        EditarEmpleadoCommand = new RelayCommand<Empleado>(EditarEmpleado, CanEditarEmpleado);
        EliminarEmpleadoCommand = new AsyncRelayCommand<Empleado>(EliminarEmpleadoAsync, CanEliminarEmpleado);
        
        CargarEmpleadosCommand.ExecuteAsync(null);
    }

    public IEnumerable<Empleado> Empleados
    {
        get => _empleadosFiltrados;
        set => SetProperty(ref _empleadosFiltrados, value);
    }

    public Empleado? EmpleadoSeleccionado
    {
        get => _empleadoSeleccionado;
        set
        {
            SetProperty(ref _empleadoSeleccionado, value);
            EditarEmpleadoCommand.NotifyCanExecuteChanged();
            EliminarEmpleadoCommand.NotifyCanExecuteChanged();
        }
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            SetProperty(ref _textoBusqueda, value);
            FiltrarEmpleados();
        }
    }

    public IAsyncRelayCommand CargarEmpleadosCommand { get; }
    public IRelayCommand CrearEmpleadoCommand { get; }
    public IRelayCommand<Empleado> EditarEmpleadoCommand { get; }
    public IAsyncRelayCommand<Empleado> EliminarEmpleadoCommand { get; }

    public event EventHandler<Empleado>? EmpleadoCreado;
    public event EventHandler<Empleado>? EmpleadoEditado;

    private async Task CargarEmpleadosAsync()
    {
        IsLoading = true;
        try
        {
            _empleados = await _empleadoService.GetAllAsync();
            FiltrarEmpleados();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar empleados: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CrearEmpleado()
    {
        var nuevoEmpleado = new Empleado
        {
            Estado = EstadoEmpleado.Activo,
            FechaContratacion = DateTime.Now
        };
        EmpleadoCreado?.Invoke(this, nuevoEmpleado);
    }

    private void EditarEmpleado(Empleado? empleado)
    {
        if (empleado != null)
        {
            EmpleadoEditado?.Invoke(this, empleado);
        }
    }

    private bool CanEditarEmpleado(Empleado? empleado) => empleado != null;

    private async Task EliminarEmpleadoAsync(Empleado? empleado)
    {
        if (empleado == null) return;

        IsLoading = true;
        try
        {
            await _empleadoService.DeleteAsync(empleado.Id);
            await CargarEmpleadosAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al eliminar empleado: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanEliminarEmpleado(Empleado? empleado) => empleado != null;

    private void FiltrarEmpleados()
    {
        if (string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            Empleados = _empleados;
            return;
        }

        Empleados = SearchHelper.FilterByText(
            _empleados,
            TextoBusqueda,
            e => e.Nombre,
            e => e.Apellidos,
            e => e.Email,
            e => e.Departamento,
            e => e.Cargo
        );
    }
}
