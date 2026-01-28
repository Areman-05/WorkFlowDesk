using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class ClienteFormViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;
    private Cliente _cliente;
    private bool _esNuevo;

    public ClienteFormViewModel(IClienteService clienteService, Cliente? cliente = null)
    {
        _clienteService = clienteService;
        _cliente = cliente ?? new Cliente
        {
            Activo = true,
            FechaCreacion = DateTime.Now
        };
        _esNuevo = cliente == null;

        GuardarCommand = new AsyncRelayCommand(GuardarAsync, CanGuardar);
        CancelarCommand = new RelayCommand(Cancelar);
    }

    public string Titulo => _esNuevo ? "Nuevo Cliente" : "Editar Cliente";

    public string Nombre
    {
        get => _cliente.Nombre;
        set
        {
            _cliente.Nombre = value;
            OnPropertyChanged();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Email
    {
        get => _cliente.Email;
        set
        {
            _cliente.Email = value;
            OnPropertyChanged();
            ValidarEmail();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Telefono
    {
        get => _cliente.Telefono;
        set
        {
            _cliente.Telefono = value;
            OnPropertyChanged();
        }
    }

    public string Direccion
    {
        get => _cliente.Direccion;
        set
        {
            _cliente.Direccion = value;
            OnPropertyChanged();
        }
    }

    public string Empresa
    {
        get => _cliente.Empresa;
        set
        {
            _cliente.Empresa = value;
            OnPropertyChanged();
        }
    }

    public bool Activo
    {
        get => _cliente.Activo;
        set
        {
            _cliente.Activo = value;
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
               !string.IsNullOrWhiteSpace(Email) &&
               ValidationHelper.IsValidEmail(Email);
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

    private async Task GuardarAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            if (_esNuevo)
            {
                await _clienteService.CreateAsync(_cliente);
            }
            else
            {
                await _clienteService.UpdateAsync(_cliente);
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
