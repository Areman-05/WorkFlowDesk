using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class ClientesViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;
    private IEnumerable<Cliente> _clientes = new List<Cliente>();
    private IEnumerable<Cliente> _clientesFiltrados = new List<Cliente>();
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
        get => _clientesFiltrados;
        set => SetProperty(ref _clientesFiltrados, value);
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
            _clientes = await _clienteService.GetActivosAsync();
            FiltrarClientes();
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

        var resultado = MessageBox.Show(
            $"¿Está seguro de que desea eliminar al cliente '{cliente.Nombre}'?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
            return;

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
        if (string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            Clientes = _clientes;
            return;
        }

        Clientes = SearchHelper.FilterByText(
            _clientes,
            TextoBusqueda,
            c => c.Nombre,
            c => c.Empresa,
            c => c.Email,
            c => c.Telefono
        );
    }
}
