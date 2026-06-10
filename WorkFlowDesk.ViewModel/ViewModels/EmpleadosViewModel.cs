using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Logging;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de listado y gestión de empleados.</summary>
public class EmpleadosViewModel : ListViewModelBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IExportService _exportService;
    private readonly PaginationHelper _paginacion = new();
    private IEnumerable<Empleado> _empleados = new List<Empleado>();
    private IEnumerable<Empleado> _empleadosFiltrados = new List<Empleado>();
    private List<Empleado> _resultadoFiltrado = new();
    private Empleado? _empleadoSeleccionado;
    private string _textoBusqueda = string.Empty;

    /// <summary>Construye el ViewModel e inicia la carga de empleados.</summary>
    public EmpleadosViewModel(IEmpleadoService empleadoService, IExportService exportService)
    {
        _empleadoService = empleadoService;
        _exportService = exportService;
        CargarEmpleadosCommand = new AsyncRelayCommand(CargarEmpleadosAsync);
        CrearEmpleadoCommand = new RelayCommand(CrearEmpleado);
        EditarEmpleadoCommand = new RelayCommand<Empleado>(EditarEmpleado, CanEditarEmpleado);
        EliminarEmpleadoCommand = new AsyncRelayCommand<Empleado>(EliminarEmpleadoAsync, CanEliminarEmpleado);
        ExportarCommand = new AsyncRelayCommand(ExportarAsync);
        PaginaAnteriorCommand = new RelayCommand(IrPaginaAnterior, () => _paginacion.TienePaginaAnterior);
        PaginaSiguienteCommand = new RelayCommand(IrPaginaSiguiente, () => _paginacion.TienePaginaSiguiente);
        
        CargarEmpleadosCommand.ExecuteAsync(null);
    }

    public bool CanManage => RolePermissions.CanManageEmpleados;
    public string InfoPaginacion => _paginacion.Resumen;
    public bool CanExport => RolePermissions.CanExportData;

    public IEnumerable<Empleado> Empleados
    {
        get => _empleadosFiltrados;
        set
        {
            SetProperty(ref _empleadosFiltrados, value);
            NotificarEstadoLista();
        }
    }

    protected override int ObtenerCantidadElementos() => _empleadosFiltrados.Count();

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
    public IAsyncRelayCommand ExportarCommand { get; }
    public IRelayCommand PaginaAnteriorCommand { get; }
    public IRelayCommand PaginaSiguienteCommand { get; }

    public event EventHandler<Empleado>? EmpleadoCreado;
    public event EventHandler<string>? ExportacionCompletada;
    public event EventHandler<Empleado>? EmpleadoEditado;

    private async Task CargarEmpleadosAsync()
    {
        IsLoading = true;
        NotificarEstadoLista();
        try
        {
            _empleados = await _empleadoService.GetAllAsync();
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

    private bool CanEliminarEmpleado(Empleado? empleado) => empleado != null;

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
                e => e.Cargo).ToList();

        _paginacion.Reiniciar();
        AplicarPaginacion();
    }

    private void AplicarPaginacion()
    {
        _paginacion.TamañoPagina = Math.Max(1, AppConfig.Settings.DefaultPageSize);
        Empleados = _paginacion.Aplicar(_resultadoFiltrado).ToList();
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
}
