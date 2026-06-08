using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de clientes.</summary>
public class ClientesViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;
    private readonly IExportService _exportService;
    private IEnumerable<Cliente> _clientes = new List<Cliente>();
    private IEnumerable<Cliente> _clientesFiltrados = new List<Cliente>();
    private Cliente? _clienteSeleccionado;
    private string _textoBusqueda = string.Empty;

    /// <summary>Construye el ViewModel e inicia la carga de clientes.</summary>
    public ClientesViewModel(IClienteService clienteService, IExportService exportService)
    {
        _clienteService = clienteService;
        _exportService = exportService;
        CargarClientesCommand = new AsyncRelayCommand(CargarClientesAsync);
        CrearClienteCommand = new RelayCommand(CrearCliente);
        EditarClienteCommand = new RelayCommand<Cliente>(EditarCliente, CanEditarCliente);
        EliminarClienteCommand = new AsyncRelayCommand<Cliente>(EliminarClienteAsync, CanEliminarCliente);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        
        CargarClientesCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageClientes;
    public bool CanExport => RolePermissions.CanExportData;

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
    public IAsyncRelayCommand ExportarCommand { get; }

    public event EventHandler<Cliente>? ClienteCreado;
    public event EventHandler<Cliente>? ClienteEditado;
    public event EventHandler<string>? ExportacionCompletada;

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
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
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
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanEliminarCliente(Cliente? cliente) => cliente != null;

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(_clientesFiltrados, "clientes");
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
