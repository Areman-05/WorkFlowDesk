using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class ClientesViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;
    private IEnumerable<Cliente> _clientes = new List<Cliente>();
    private Cliente? _clienteSeleccionado;
    private string _textoBusqueda = string.Empty;

    public ClientesViewModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
        CargarClientesCommand = new AsyncRelayCommand(CargarClientesAsync);
        CrearClienteCommand = new RelayCommand(CrearCliente);
        EditarClienteCommand = new RelayCommand<Cliente>(EditarCliente, CanEditarCliente);
        EliminarClienteCommand = new AsyncRelayCommand<Cliente>(EliminarClienteAsync, CanEliminarCliente);
        
        CargarClientesCommand.ExecuteAsync(null);
    }

    public IEnumerable<Cliente> Clientes
    {
        get => _clientes;
        set => SetProperty(ref _clientes, value);
    }

    public Cliente? ClienteSeleccionado
    {
        get => _clienteSeleccionado;
        set
        {
            SetProperty(ref _clienteSeleccionado, value);
            EditarClienteCommand.NotifyCanExecuteChanged();
            EliminarClienteCommand.NotifyCanExecuteChanged();
        }
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            SetProperty(ref _textoBusqueda, value);
            FiltrarClientes();
        }
    }

    public IAsyncRelayCommand CargarClientesCommand { get; }
    public IRelayCommand CrearClienteCommand { get; }
    public IRelayCommand<Cliente> EditarClienteCommand { get; }
    public IAsyncRelayCommand<Cliente> EliminarClienteCommand { get; }

    public event EventHandler<Cliente>? ClienteCreado;
    public event EventHandler<Cliente>? ClienteEditado;

    private async Task CargarClientesAsync()
    {
        IsLoading = true;
        try
        {
            Clientes = await _clienteService.GetActivosAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar clientes: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CrearCliente()
    {
        var nuevoCliente = new Cliente
        {
            Activo = true,
            FechaCreacion = DateTime.Now
        };
        ClienteCreado?.Invoke(this, nuevoCliente);
    }

    private void EditarCliente(Cliente? cliente)
    {
        if (cliente != null)
        {
            ClienteEditado?.Invoke(this, cliente);
        }
    }

    private bool CanEditarCliente(Cliente? cliente) => cliente != null;

    private async Task EliminarClienteAsync(Cliente? cliente)
    {
        if (cliente == null) return;

        IsLoading = true;
        try
        {
            await _clienteService.DeleteAsync(cliente.Id);
            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al eliminar cliente: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanEliminarCliente(Cliente? cliente) => cliente != null;

    private void FiltrarClientes()
    {
        // La lógica de filtrado se puede mejorar más adelante
    }
}
