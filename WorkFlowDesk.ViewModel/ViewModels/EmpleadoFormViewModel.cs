using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class EmpleadoFormViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;
    private Empleado _empleado;
    private bool _esNuevo;

    public EmpleadoFormViewModel(IEmpleadoService empleadoService, Empleado? empleado = null)
    {
        _empleadoService = empleadoService;
        _empleado = empleado ?? new Empleado
        {
            Estado = EstadoEmpleado.Activo,
            FechaContratacion = DateTime.Now
        };
        _esNuevo = empleado == null;

        GuardarCommand = new AsyncRelayCommand(GuardarAsync, CanGuardar);
        CancelarCommand = new RelayCommand(Cancelar);
    }

    public string Titulo => _esNuevo ? "Nuevo Empleado" : "Editar Empleado";

    public string Nombre
    {
        get => _empleado.Nombre;
        set
        {
            _empleado.Nombre = value;
            OnPropertyChanged();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Apellidos
    {
        get => _empleado.Apellidos;
        set
        {
            _empleado.Apellidos = value;
            OnPropertyChanged();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Email
    {
        get => _empleado.Email;
        set
        {
            _empleado.Email = value;
            OnPropertyChanged();
            ValidarEmail();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    private void ValidarEmail()
    {
        if (!string.IsNullOrWhiteSpace(Email) && !ValidationHelper.IsValidEmail(Email))
        {
            ErrorMessage = "El formato del email no es válido";
        }
        else if (string.IsNullOrWhiteSpace(ErrorMessage) || ErrorMessage.Contains("email"))
        {
            ErrorMessage = null;
        }
    }

    public string Telefono
    {
        get => _empleado.Telefono;
        set
        {
            _empleado.Telefono = value;
            OnPropertyChanged();
        }
    }

    public string Departamento
    {
        get => _empleado.Departamento;
        set
        {
            _empleado.Departamento = value;
            OnPropertyChanged();
        }
    }

    public string Cargo
    {
        get => _empleado.Cargo;
        set
        {
            _empleado.Cargo = value;
            OnPropertyChanged();
        }
    }

    public EstadoEmpleado Estado
    {
        get => _empleado.Estado;
        set
        {
            _empleado.Estado = value;
            OnPropertyChanged();
        }
    }

    public DateTime FechaContratacion
    {
        get => _empleado.FechaContratacion;
        set
        {
            _empleado.FechaContratacion = value;
            OnPropertyChanged();
        }
    }

    public IAsyncRelayCommand GuardarCommand { get; }
    public IRelayCommand CancelarCommand { get; }

    public event EventHandler? Guardado;
    public event EventHandler? Cancelado;

    private bool CanGuardar()
    {
        return !string.IsNullOrWhiteSpace(Nombre) &&
               !string.IsNullOrWhiteSpace(Apellidos) &&
               !string.IsNullOrWhiteSpace(Email);
    }

    private async Task GuardarAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            if (_esNuevo)
            {
                await _empleadoService.CreateAsync(_empleado);
            }
            else
            {
                await _empleadoService.UpdateAsync(_empleado);
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
