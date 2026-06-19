using System.Globalization;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Helpers;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de clientes.</summary>
public class ClientesViewModel : ListViewModelBase, ISearchableViewModel
{
    private readonly IClienteService _clienteService;
    private readonly IExportService _exportService;
    private readonly PaginationHelper _paginacion = new();
    private IEnumerable<Cliente> _clientes = new List<Cliente>();
    private IEnumerable<ClienteListItem> _clientesFiltrados = new List<ClienteListItem>();
    private List<Cliente> _resultadoFiltrado = new();
    private ClienteListItem? _clienteSeleccionado;
    private string _textoBusqueda = string.Empty;
    private int _totalClientes;
    private int _activos;
    private int _proyectosActivos;
    private string _crecimientoTexto = "+0%";
    private string _perspectivaNegocio = string.Empty;
    private string _accionSugerida = string.Empty;

    public ClientesViewModel(IClienteService clienteService, IExportService exportService)
    {
        _clienteService = clienteService;
        _exportService = exportService;

        CargarClientesCommand = new AsyncRelayCommand(CargarClientesAsync);
        CrearClienteCommand = new RelayCommand(CrearCliente);
        EditarClienteCommand = new RelayCommand<ClienteListItem>(EditarCliente, CanEditarCliente);
        EliminarClienteCommand = new AsyncRelayCommand<ClienteListItem>(EliminarClienteAsync, CanEliminarCliente);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);

        CargarClientesCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageClientes;
    public bool CanExport => RolePermissions.CanExportData;

    public string InfoPaginacion => _paginacion.TotalItems == 0
        ? "Sin clientes"
        : $"{_paginacion.TotalItems} clientes";

    public int TotalClientes
    {
        get => _totalClientes;
        private set => SetProperty(ref _totalClientes, value);
    }

    public int Activos
    {
        get => _activos;
        private set => SetProperty(ref _activos, value);
    }

    public int ProyectosActivos
    {
        get => _proyectosActivos;
        private set => SetProperty(ref _proyectosActivos, value);
    }

    public string CrecimientoTexto
    {
        get => _crecimientoTexto;
        private set => SetProperty(ref _crecimientoTexto, value);
    }

    public string PerspectivaNegocio
    {
        get => _perspectivaNegocio;
        private set => SetProperty(ref _perspectivaNegocio, value);
    }

    public string AccionSugerida
    {
        get => _accionSugerida;
        private set => SetProperty(ref _accionSugerida, value);
    }

    public IEnumerable<ClienteListItem> Clientes
    {
        get => _clientesFiltrados;
        private set
        {
            SetProperty(ref _clientesFiltrados, value);
            NotificarEstadoLista();
            OnPropertyChanged(nameof(InfoPaginacion));
        }
    }

    protected override int ObtenerCantidadElementos() => _clientesFiltrados.Count();

    public ClienteListItem? ClienteSeleccionado
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
    public IRelayCommand<ClienteListItem> EditarClienteCommand { get; }
    public IAsyncRelayCommand<ClienteListItem> EliminarClienteCommand { get; }
    public IAsyncRelayCommand ExportarCommand { get; }

    public event EventHandler<Cliente>? ClienteCreado;
    public event EventHandler<Cliente>? ClienteEditado;
    public event EventHandler<string>? ExportacionCompletada;

    private async Task CargarClientesAsync()
    {
        IsLoading = true;
        NotificarEstadoLista();
        try
        {
            _clientes = (await _clienteService.GetAllAsync()).ToList();
            ActualizarEstadisticas();
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
            NotificarEstadoLista();
        }
    }

    private void ActualizarEstadisticas()
    {
        var lista = _clientes.ToList();
        TotalClientes = lista.Count;
        Activos = lista.Count(c => c.Activo);

        var proyectos = lista.SelectMany(c => c.Proyectos).ToList();
        ProyectosActivos = proyectos.Count(p =>
            p.Estado is EstadoProyecto.EnProgreso or EstadoProyecto.Planificacion);

        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var inicioMesAnterior = inicioMes.AddMonths(-1);
        var nuevosEsteMes = lista.Count(c => c.FechaCreacion >= inicioMes);
        var nuevosMesAnterior = lista.Count(c =>
            c.FechaCreacion >= inicioMesAnterior && c.FechaCreacion < inicioMes);

        var delta = nuevosEsteMes - nuevosMesAnterior;
        var pct = nuevosMesAnterior == 0
            ? (nuevosEsteMes > 0 ? 100 : 0)
            : (int)Math.Round(100.0 * delta / nuevosMesAnterior);
        CrecimientoTexto = pct >= 0 ? $"+{pct}%" : $"{pct}%";

        var sectorTop = lista
            .GroupBy(c => ClienteSectorHelper.InferirSector(c.Empresa, c.Nombre))
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? "Servicios";

        PerspectivaNegocio = nuevosEsteMes > 0
            ? $"Has añadido {nuevosEsteMes} nuevo(s) cliente(s) este mes. El sector de {sectorTop} concentra buena parte de tu cartera actual."
            : "Mantén el contacto con tu cartera activa. Revisa los clientes con proyectos en curso para detectar oportunidades.";

        var clientePausado = lista.FirstOrDefault(c =>
            c.Activo && c.Proyectos.Any(p => p.Estado == EstadoProyecto.EnPausa));

        AccionSugerida = clientePausado != null
            ? $"{ObtenerNombreDisplay(clientePausado)} tiene un proyecto pausado. Considera enviar una propuesta de reactivación."
            : "No hay acciones urgentes. Tu cartera de clientes está al día.";
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

    private void EditarCliente(ClienteListItem? item)
    {
        if (item != null)
            ClienteEditado?.Invoke(this, item.Cliente);
    }

    private bool CanEditarCliente(ClienteListItem? item) => item != null;

    private async Task EliminarClienteAsync(ClienteListItem? item)
    {
        if (item == null) return;

        var cliente = item.Cliente;
        if (!SolicitarConfirmacion(
                $"¿Está seguro de que desea desactivar al cliente '{ObtenerNombreDisplay(cliente)}'?",
                "Confirmar desactivación"))
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

    private bool CanEliminarCliente(ClienteListItem? item) => item != null;

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(_resultadoFiltrado, "clientes");
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
        var consulta = _clientes.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            consulta = SearchHelper.FilterByText(
                consulta,
                TextoBusqueda,
                c => c.Nombre,
                c => c.Empresa,
                c => c.Email,
                c => c.Telefono,
                c => ClienteSectorHelper.InferirSector(c.Empresa, c.Nombre));
        }

        _resultadoFiltrado = consulta.ToList();
        _paginacion.Reiniciar();
        AplicarPaginacion();
    }

    private void AplicarPaginacion()
    {
        _paginacion.TamañoPagina = Math.Max(1, _resultadoFiltrado.Count);
        var pagina = _paginacion.Aplicar(_resultadoFiltrado).Select(MapToListItem).ToList();
        Clientes = pagina;
        OnPropertyChanged(nameof(InfoPaginacion));
    }

    private static ClienteListItem MapToListItem(Cliente cliente)
    {
        var display = ObtenerNombreDisplay(cliente);
        var sectorVisual = ClienteSectorHelper.Resolver(cliente.Empresa, cliente.Nombre);
        var activos = cliente.Proyectos.Count(p =>
            p.Estado is EstadoProyecto.EnProgreso or EstadoProyecto.Planificacion);
        var completados = cliente.Proyectos.Count(p => p.Estado == EstadoProyecto.Completado);

        var (count, etiqueta) = activos > 0
            ? (activos, "en curso")
            : completados > 0
                ? (completados, "finalizado")
                : (cliente.Proyectos.Count, "proyectos");

        return new ClienteListItem
        {
            Cliente = cliente,
            NombreDisplay = display,
            CodigoId = $"ID: CL-{cliente.Id:D4}",
            Sector = sectorVisual.Sector,
            SectorIcon = sectorVisual.IconGlyph,
            SectorIconFondo = sectorVisual.Fondo,
            SectorIconColor = sectorVisual.IconColor,
            ContactoNombre = ObtenerContactoNombre(cliente, display),
            ContactoEmail = string.IsNullOrWhiteSpace(cliente.Email) ? "—" : cliente.Email,
            EstadoTexto = cliente.Activo ? "Activo" : "Inactivo",
            EstadoFondo = cliente.Activo ? "#DCFCE7" : "#F1F5F9",
            EstadoTextoColor = cliente.Activo ? "#15803D" : "#64748B",
            EstadoDotColor = cliente.Activo ? "#22C55E" : "#94A3B8",
            ProyectosCount = count,
            ProyectosEtiqueta = etiqueta
        };
    }

    private static string ObtenerNombreDisplay(Cliente cliente) =>
        !string.IsNullOrWhiteSpace(cliente.Empresa) ? cliente.Empresa.Trim() : cliente.Nombre.Trim();

    private static string ObtenerContactoNombre(Cliente cliente, string display)
    {
        if (!string.IsNullOrWhiteSpace(cliente.Nombre) &&
            !cliente.Nombre.Equals(display, StringComparison.OrdinalIgnoreCase))
            return cliente.Nombre.Trim();

        if (string.IsNullOrWhiteSpace(cliente.Email))
            return "Contacto comercial";

        var local = cliente.Email.Split('@')[0]
            .Replace('.', ' ')
            .Replace('_', ' ')
            .Trim();
        return CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(local);
    }
}
