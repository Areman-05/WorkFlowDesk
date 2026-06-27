using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Logging;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de empleados.</summary>
public class EmpleadosViewModel : ListViewModelBase, ISearchableViewModel, IListToolbarProvider
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IExportService _exportService;
    private readonly PaginationHelper _paginacion = new();
    private IEnumerable<Empleado> _empleados = new List<Empleado>();
    private IEnumerable<EmpleadoListItem> _empleadosFiltrados = new List<EmpleadoListItem>();
    private List<Empleado> _resultadoFiltrado = new();
    private EmpleadoListItem? _empleadoSeleccionado;
    private string _textoBusqueda = string.Empty;
    private int _totalEquipo;
    private int _activosHoy;
    private int _ausencias;
    private int _nuevosEsteMes;

    /// <summary>Construye el ViewModel e inicia la carga de empleados.</summary>
    public EmpleadosViewModel(IEmpleadoService empleadoService, IExportService exportService)
    {
        _empleadoService = empleadoService;
        _exportService = exportService;
        CargarEmpleadosCommand = new AsyncRelayCommand(CargarEmpleadosAsync);
        CrearEmpleadoCommand = new RelayCommand(CrearEmpleado);
        EditarEmpleadoCommand = new RelayCommand<EmpleadoListItem>(EditarEmpleado, CanEditarEmpleado);
        EliminarEmpleadoCommand = new AsyncRelayCommand<EmpleadoListItem>(EliminarEmpleadoAsync, CanEliminarEmpleado);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        PaginaAnteriorCommand = new RelayCommand(IrPaginaAnterior, () => _paginacion.TienePaginaAnterior);
        PaginaSiguienteCommand = new RelayCommand(IrPaginaSiguiente, () => _paginacion.TienePaginaSiguiente);

        CargarEmpleadosCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageEmpleados;
    public bool CanExport => RolePermissions.CanExportData;

    public string InfoPaginacion => _paginacion.TotalItems == 0
        ? "Sin empleados"
        : $"{_paginacion.TotalItems} empleados";

    public int TotalEquipo
    {
        get => _totalEquipo;
        private set => SetProperty(ref _totalEquipo, value);
    }

    public int ActivosHoy
    {
        get => _activosHoy;
        private set => SetProperty(ref _activosHoy, value);
    }

    public int Ausencias
    {
        get => _ausencias;
        private set => SetProperty(ref _ausencias, value);
    }

    public int NuevosEsteMes
    {
        get => _nuevosEsteMes;
        private set => SetProperty(ref _nuevosEsteMes, value);
    }

    public IEnumerable<EmpleadoListItem> Empleados
    {
        get => _empleadosFiltrados;
        private set
        {
            SetProperty(ref _empleadosFiltrados, value);
            NotificarEstadoLista();
            OnPropertyChanged(nameof(InfoPaginacion));
        }
    }

    protected override int ObtenerCantidadElementos() => _empleadosFiltrados.Count();

    public EmpleadoListItem? EmpleadoSeleccionado
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
    public IRelayCommand<EmpleadoListItem> EditarEmpleadoCommand { get; }
    public IAsyncRelayCommand<EmpleadoListItem> EliminarEmpleadoCommand { get; }
    public IAsyncRelayCommand ExportarCommand { get; }
    public IRelayCommand PaginaAnteriorCommand { get; }
    public IRelayCommand PaginaSiguienteCommand { get; }

    public event EventHandler<Empleado>? EmpleadoCreado;
    public event EventHandler<string>? ExportacionCompletada;
    public event EventHandler<Empleado>? EmpleadoEditado;

    public bool ToolbarExportVisible => CanExport;
    public bool ToolbarCreateVisible => CanManage;
    public string ToolbarCreateLabel => "Nuevo empleado";
    public IAsyncRelayCommand? ToolbarExportCommand => ExportarCommand;
    public IRelayCommand? ToolbarCreateCommand => CrearEmpleadoCommand;

    private async Task CargarEmpleadosAsync()
    {
        IsLoading = true;
        NotificarEstadoLista();
        try
        {
            _empleados = await _empleadoService.GetAllAsync();
            ActualizarEstadisticas();
            FiltrarEmpleados();
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
            SimpleLogger.LogError("Error al cargar empleados", ex);
        }
        finally
        {
            IsLoading = false;
            NotificarEstadoLista();
        }
    }

    private void ActualizarEstadisticas()
    {
        var lista = _empleados.ToList();
        var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        TotalEquipo = lista.Count;
        ActivosHoy = lista.Count(e => e.Estado == EstadoEmpleado.Activo);
        Ausencias = lista.Count(e => e.Estado is EstadoEmpleado.Vacaciones or EstadoEmpleado.Inactivo);
        NuevosEsteMes = lista.Count(e => e.FechaContratacion >= inicioMes);
    }

    private void CrearEmpleado()
    {
        var nuevoEmpleado = new Empleado
        {
            Estado = EstadoEmpleado.Activo,
            FechaContratacion = DateTime.Now,
            AvatarIndex = _empleados.Count() % AvatarCatalog.Count
        };
        EmpleadoCreado?.Invoke(this, nuevoEmpleado);
    }

    private void EditarEmpleado(EmpleadoListItem? item)
    {
        if (item != null)
            EmpleadoEditado?.Invoke(this, item.Empleado);
    }

    private bool CanEditarEmpleado(EmpleadoListItem? item) => item != null;

    private async Task EliminarEmpleadoAsync(EmpleadoListItem? item)
    {
        if (item == null) return;

        var empleado = item.Empleado;
        if (!SolicitarConfirmacion(
                $"¿Está seguro de que desea eliminar al empleado {empleado.Nombre} {empleado.Apellidos}?",
                "Confirmar eliminación"))
            return;

        IsLoading = true;
        try
        {
            await _empleadoService.DeleteAsync(empleado.Id);
            await CargarEmpleadosAsync();
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
            SimpleLogger.LogError("Error al eliminar empleado", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanEliminarEmpleado(EmpleadoListItem? item) => item != null;

    private async Task ExportarAsync()
    {
        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(_resultadoFiltrado, "empleados");
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

    private void FiltrarEmpleados()
    {
        _resultadoFiltrado = string.IsNullOrWhiteSpace(TextoBusqueda)
            ? _empleados.ToList()
            : SearchHelper.FilterByText(
                _empleados,
                TextoBusqueda,
                e => e.Nombre,
                e => e.Apellidos,
                e => e.Email,
                e => e.Departamento,
                e => e.Cargo,
                e => e.Usuario?.NombreUsuario ?? string.Empty).ToList();

        _paginacion.Reiniciar();
        AplicarPaginacion();
    }

    private void AplicarPaginacion()
    {
        _paginacion.TamañoPagina = Math.Max(1, _resultadoFiltrado.Count);
        var pagina = _paginacion.Aplicar(_resultadoFiltrado).Select(MapToListItem).ToList();
        Empleados = pagina;
        OnPropertyChanged(nameof(InfoPaginacion));
        PaginaAnteriorCommand.NotifyCanExecuteChanged();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
    }

    private void IrPaginaAnterior()
    {
        _paginacion.PaginaAnterior();
        AplicarPaginacion();
    }

    private void IrPaginaSiguiente()
    {
        _paginacion.PaginaSiguiente();
        AplicarPaginacion();
    }

    private static EmpleadoListItem MapToListItem(Empleado empleado)
    {
        var avatarIndex = empleado.AvatarIndex;
        if (avatarIndex == 0 && empleado.Id > 1)
            avatarIndex = (empleado.Id - 1) % AvatarCatalog.Count;

        avatarIndex = Math.Clamp(avatarIndex, 0, AvatarCatalog.Count - 1);

        var (estadoTexto, estadoFondo, estadoColor) = empleado.Estado switch
        {
            EstadoEmpleado.Activo => ("Activo", "#E6FCF5", "#0CA678"),
            EstadoEmpleado.Vacaciones => ("Ausente", "#FFF4E6", "#F76707"),
            EstadoEmpleado.Inactivo => ("Ausente", "#FFF4E6", "#F76707"),
            EstadoEmpleado.Baja => ("Baja", "#FFEBEB", "#BA1A1A"),
            _ => ("Ausente", "#FFF4E6", "#F76707")
        };

        var usuario = empleado.Usuario?.NombreUsuario;
        if (string.IsNullOrWhiteSpace(usuario))
            usuario = $"@{empleado.Email.Split('@')[0].Replace('.', '_')}";

        return new EmpleadoListItem
        {
            Empleado = empleado,
            AvatarIndex = avatarIndex,
            NombreCompleto = $"{empleado.Nombre} {empleado.Apellidos}",
            Iniciales = GetIniciales(empleado.Nombre, empleado.Apellidos),
            CodigoId = $"ID: #{44000 + empleado.Id}",
            NombreUsuario = usuario.StartsWith('@') ? usuario : $"@{usuario}",
            Email = empleado.Email,
            Rol = empleado.Usuario?.Rol?.Nombre ?? "Empleado",
            Cargo = string.IsNullOrWhiteSpace(empleado.Cargo) ? empleado.Departamento : empleado.Cargo,
            EstadoTexto = estadoTexto,
            EstadoFondo = estadoFondo,
            EstadoTextoColor = estadoColor
        };
    }

    private static string GetIniciales(string nombre, string apellidos)
    {
        var n = string.IsNullOrWhiteSpace(nombre) ? "?" : nombre.Trim()[0].ToString();
        var a = string.IsNullOrWhiteSpace(apellidos) ? string.Empty : apellidos.Trim()[0].ToString();
        return $"{n}{a}".ToUpperInvariant();
    }
}
