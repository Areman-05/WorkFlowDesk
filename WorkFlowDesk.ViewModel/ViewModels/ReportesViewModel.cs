using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de reportes y estadísticas.</summary>
public class ReportesViewModel : ViewModelBase
{
    private const double CircunferenciaDonut = 251.2;

    private readonly IReporteService _reporteService;
    private readonly IExportService _exportService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IProyectoService _proyectoService;
    private readonly ITareaService _tareaService;

    private Dictionary<string, int> _estadisticasEmpleados = new();
    private Dictionary<string, int> _estadisticasProyectos = new();
    private Dictionary<string, int> _estadisticasTareas = new();
    private Dictionary<string, int> _estadisticasClientes = new();
    private IReadOnlyList<ActividadDiaItem> _productividadVisible = Array.Empty<ActividadDiaItem>();
    private IReadOnlyList<ActividadDiaItem> _productividad7Dias = Array.Empty<ActividadDiaItem>();
    private IReadOnlyList<ActividadDiaItem> _productividad30Dias = Array.Empty<ActividadDiaItem>();
    private int _eficienciaPorcentaje;
    private int _proyectosCompletados;
    private int _proyectosEnProgreso;
    private int _proyectosPendientes;
    private double _donutCompletadosArco;
    private double _donutEnProgresoArco;
    private double _donutPendientesArco;
    private double _donutEnProgresoOffset;
    private double _donutPendientesOffset;
    private string _crecimientoMensualTexto = "+0%";
    private string _promedioEntregaTexto = "—";
    private string _colaboracionTexto = "—";
    private bool _filtro7DiasActivo = true;

    private bool _cargaInicialCompleta;

    public ReportesViewModel(
        IReporteService reporteService,
        IExportService exportService,
        IEmpleadoService empleadoService,
        IProyectoService proyectoService,
        ITareaService tareaService)
    {
        _reporteService = reporteService;
        _exportService = exportService;
        _empleadoService = empleadoService;
        _proyectoService = proyectoService;
        _tareaService = tareaService;

        CargarReportesCommand = new AsyncRelayCommand(CargarReportesAsync);
        ExportarResumenCommand = new AsyncRelayCommand(ExportarResumenAsync);
        ExportarEmpleadosCommand = new AsyncRelayCommand(ExportarEmpleadosAsync);
        ExportarProyectosCommand = new AsyncRelayCommand(ExportarProyectosAsync);
        ExportarTareasCommand = new AsyncRelayCommand(ExportarTareasAsync);
        Filtrar7DiasCommand = new RelayCommand(() => AplicarFiltroProductividad(true));
        Filtrar30DiasCommand = new RelayCommand(() => AplicarFiltroProductividad(false));

        CargarReportesCommand.ExecuteAsync(null);
    }

    public bool CanExport => RolePermissions.CanExportData;
    public bool Filtro7DiasActivo => _filtro7DiasActivo;
    public bool Filtro30DiasActivo => !_filtro7DiasActivo;

    public int EficienciaPorcentaje
    {
        get => _eficienciaPorcentaje;
        private set => SetProperty(ref _eficienciaPorcentaje, value);
    }

    public int ProyectosCompletados
    {
        get => _proyectosCompletados;
        private set => SetProperty(ref _proyectosCompletados, value);
    }

    public int ProyectosEnProgreso
    {
        get => _proyectosEnProgreso;
        private set => SetProperty(ref _proyectosEnProgreso, value);
    }

    public int ProyectosPendientes
    {
        get => _proyectosPendientes;
        private set => SetProperty(ref _proyectosPendientes, value);
    }

    public double DonutCompletadosArco
    {
        get => _donutCompletadosArco;
        private set => SetProperty(ref _donutCompletadosArco, value);
    }

    public double DonutEnProgresoArco
    {
        get => _donutEnProgresoArco;
        private set => SetProperty(ref _donutEnProgresoArco, value);
    }

    public double DonutPendientesArco
    {
        get => _donutPendientesArco;
        private set => SetProperty(ref _donutPendientesArco, value);
    }

    public double DonutEnProgresoOffset
    {
        get => _donutEnProgresoOffset;
        private set => SetProperty(ref _donutEnProgresoOffset, value);
    }

    public double DonutPendientesOffset
    {
        get => _donutPendientesOffset;
        private set => SetProperty(ref _donutPendientesOffset, value);
    }

    public string CrecimientoMensualTexto
    {
        get => _crecimientoMensualTexto;
        private set => SetProperty(ref _crecimientoMensualTexto, value);
    }

    public string PromedioEntregaTexto
    {
        get => _promedioEntregaTexto;
        private set => SetProperty(ref _promedioEntregaTexto, value);
    }

    public string ColaboracionTexto
    {
        get => _colaboracionTexto;
        private set => SetProperty(ref _colaboracionTexto, value);
    }

    public bool MostrarDonut => DonutCompletadosArco + DonutEnProgresoArco + DonutPendientesArco > 0;

    public IReadOnlyList<ActividadDiaItem> ProductividadVisible
    {
        get => _productividadVisible;
        private set => SetProperty(ref _productividadVisible, value);
    }

    public Dictionary<string, int> EstadisticasEmpleados
    {
        get => _estadisticasEmpleados;
        private set => SetProperty(ref _estadisticasEmpleados, value);
    }

    public Dictionary<string, int> EstadisticasProyectos
    {
        get => _estadisticasProyectos;
        private set => SetProperty(ref _estadisticasProyectos, value);
    }

    public Dictionary<string, int> EstadisticasTareas
    {
        get => _estadisticasTareas;
        private set => SetProperty(ref _estadisticasTareas, value);
    }

    public Dictionary<string, int> EstadisticasClientes
    {
        get => _estadisticasClientes;
        private set => SetProperty(ref _estadisticasClientes, value);
    }

    public IAsyncRelayCommand CargarReportesCommand { get; }
    public IAsyncRelayCommand ExportarResumenCommand { get; }
    public IAsyncRelayCommand ExportarEmpleadosCommand { get; }
    public IAsyncRelayCommand ExportarProyectosCommand { get; }
    public IAsyncRelayCommand ExportarTareasCommand { get; }
    public IRelayCommand Filtrar7DiasCommand { get; }
    public IRelayCommand Filtrar30DiasCommand { get; }

    public event EventHandler<string>? ExportacionCompletada;
    public event EventHandler? DatosActualizados;

    private async Task CargarReportesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        var exito = false;
        try
        {
            EstadisticasEmpleados = await _reporteService.ObtenerEstadisticasEmpleadosAsync();
            EstadisticasProyectos = await _reporteService.ObtenerEstadisticasProyectosAsync();
            EstadisticasTareas = await _reporteService.ObtenerEstadisticasTareasAsync();
            EstadisticasClientes = await _reporteService.ObtenerEstadisticasClientesAsync();

            var proyectos = (await _proyectoService.GetAllAsync()).ToList();
            var tareas = (await _tareaService.GetAllAsync()).ToList();

            CalcularDonutProyectos(proyectos);
            CalcularProductividad(tareas);
            CalcularMetricasResumen(proyectos, tareas);
            AplicarFiltroProductividad(_filtro7DiasActivo);
            exito = true;
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
        finally
        {
            IsLoading = false;
            if (exito)
            {
                if (_cargaInicialCompleta)
                    DatosActualizados?.Invoke(this, EventArgs.Empty);
                _cargaInicialCompleta = true;
            }
        }
    }

    private void CalcularDonutProyectos(List<Proyecto> proyectos)
    {
        var completados = proyectos.Count(p => p.Estado == EstadoProyecto.Completado);
        var enProgreso = proyectos.Count(p => p.Estado is EstadoProyecto.EnProgreso or EstadoProyecto.EnPausa);
        var pendientes = proyectos.Count(p => p.Estado == EstadoProyecto.Planificacion);
        var total = completados + enProgreso + pendientes;

        ProyectosCompletados = completados;
        ProyectosEnProgreso = enProgreso;
        ProyectosPendientes = pendientes;
        EficienciaPorcentaje = total == 0 ? 0 : (int)Math.Round(100.0 * completados / total);

        if (total == 0)
        {
            DonutCompletadosArco = 0;
            DonutEnProgresoArco = 0;
            DonutPendientesArco = 0;
            DonutEnProgresoOffset = 0;
            DonutPendientesOffset = 0;
            OnPropertyChanged(nameof(MostrarDonut));
            return;
        }

        DonutCompletadosArco = CircunferenciaDonut * completados / total;
        DonutEnProgresoArco = CircunferenciaDonut * enProgreso / total;
        DonutPendientesArco = CircunferenciaDonut * pendientes / total;
        DonutEnProgresoOffset = -DonutCompletadosArco;
        DonutPendientesOffset = -(DonutCompletadosArco + DonutEnProgresoArco);
        OnPropertyChanged(nameof(MostrarDonut));
    }

    private void CalcularProductividad(List<Tarea> tareas)
    {
        var hoy = DateTime.Today;
        var inicio7 = hoy.AddDays(-6);
        var inicio30 = hoy.AddDays(-29);

        _productividad7Dias = ConstruirBarrasDiarias(tareas, inicio7, hoy);
        _productividad30Dias = ConstruirBarrasSemanales(tareas, inicio30, hoy);
    }

    private static IReadOnlyList<ActividadDiaItem> ConstruirBarrasDiarias(List<Tarea> tareas, DateTime inicio, DateTime fin)
    {
        var dias = Enumerable.Range(0, (fin - inicio).Days + 1)
            .Select(i => inicio.AddDays(i))
            .ToList();

        var valores = dias.Select(dia =>
            tareas.Count(t =>
                t.Estado == EstadoTarea.Completada &&
                (t.FechaFin?.Date ?? t.FechaCreacion.Date) == dia.Date)).ToList();

        return CrearBarras(dias, valores, d => d.ToString("ddd", System.Globalization.CultureInfo.GetCultureInfo("es-ES")), fin);
    }

    private static IReadOnlyList<ActividadDiaItem> ConstruirBarrasSemanales(List<Tarea> tareas, DateTime inicio, DateTime fin)
    {
        var semanas = Enumerable.Range(0, 4)
            .Select(i => fin.AddDays(-7 * (3 - i)))
            .Select(finSemana => (Inicio: finSemana.AddDays(-6), Fin: finSemana))
            .ToList();

        var valores = semanas.Select(semana =>
            tareas.Count(t =>
                t.Estado == EstadoTarea.Completada &&
                (t.FechaFin?.Date ?? t.FechaCreacion.Date) >= semana.Inicio.Date &&
                (t.FechaFin?.Date ?? t.FechaCreacion.Date) <= semana.Fin.Date)).ToList();

        var etiquetas = semanas.Select((s, i) => $"S{i + 1}").ToList();
        return CrearBarrasEtiquetas(etiquetas, valores, fin);
    }

    private static IReadOnlyList<ActividadDiaItem> CrearBarras(
        IReadOnlyList<DateTime> dias,
        IReadOnlyList<int> valores,
        Func<DateTime, string> etiqueta,
        DateTime hoy)
    {
        var max = valores.DefaultIfEmpty(0).Max();
        if (max == 0) max = 1;

        return dias.Select((dia, i) => new ActividadDiaItem
        {
            Etiqueta = etiqueta(dia),
            Valor = valores[i],
            AlturaPorcentaje = Math.Max(8, valores[i] * 100.0 / max),
            EsDiaActual = dia.Date == hoy.Date
        }).ToList();
    }

    private static IReadOnlyList<ActividadDiaItem> CrearBarrasEtiquetas(
        IReadOnlyList<string> etiquetas,
        IReadOnlyList<int> valores,
        DateTime hoy)
    {
        var max = valores.DefaultIfEmpty(0).Max();
        if (max == 0) max = 1;

        return etiquetas.Select((et, i) => new ActividadDiaItem
        {
            Etiqueta = et,
            Valor = valores[i],
            AlturaPorcentaje = Math.Max(8, valores[i] * 100.0 / max),
            EsDiaActual = i == etiquetas.Count - 1
        }).ToList();
    }

    private void CalcularMetricasResumen(List<Proyecto> proyectos, List<Tarea> tareas)
    {
        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var inicioMesAnterior = inicioMes.AddMonths(-1);

        var completadasMes = tareas.Count(t =>
            t.Estado == EstadoTarea.Completada &&
            (t.FechaFin?.Date ?? t.FechaCreacion.Date) >= inicioMes);
        var completadasMesAnterior = tareas.Count(t =>
            t.Estado == EstadoTarea.Completada &&
            (t.FechaFin?.Date ?? t.FechaCreacion.Date) >= inicioMesAnterior &&
            (t.FechaFin?.Date ?? t.FechaCreacion.Date) < inicioMes);

        var delta = completadasMes - completadasMesAnterior;
        var pct = completadasMesAnterior == 0
            ? (completadasMes > 0 ? 100 : 0)
            : Math.Round(100.0 * delta / completadasMesAnterior, 1);
        CrecimientoMensualTexto = pct >= 0 ? $"+{pct}%" : $"{pct}%";

        var entregados = proyectos
            .Where(p => p.Estado == EstadoProyecto.Completado && p.FechaFin.HasValue)
            .Select(p => (p.FechaFin!.Value - p.FechaInicio).TotalDays)
            .Where(d => d >= 0)
            .ToList();

        PromedioEntregaTexto = entregados.Count == 0
            ? "—"
            : $"{entregados.Average():0.#} Días";

        var totalActivas = tareas.Count(t => t.Estado != EstadoTarea.Cancelada);
        var asignadas = tareas.Count(t => t.AsignadoId.HasValue && t.Estado != EstadoTarea.Cancelada);
        var ratio = totalActivas == 0 ? 0 : 100.0 * asignadas / totalActivas;

        ColaboracionTexto = ratio switch
        {
            >= 75 => "Alta",
            >= 40 => "Media",
            _ => "Baja"
        };
    }

    private void AplicarFiltroProductividad(bool sieteDias)
    {
        _filtro7DiasActivo = sieteDias;
        ProductividadVisible = sieteDias ? _productividad7Dias : _productividad30Dias;
        OnPropertyChanged(nameof(Filtro7DiasActivo));
        OnPropertyChanged(nameof(Filtro30DiasActivo));
    }

    private async Task ExportarResumenAsync()
    {
        IsLoading = true;
        try
        {
            var filas = ConstruirFilasExportacion();
            var path = await _exportService.ExportToCsvAsync(filas, "reportes");
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

    private async Task ExportarEmpleadosAsync() =>
        await ExportarEntidadesAsync(await _empleadoService.GetAllAsync(), "empleados");

    private async Task ExportarProyectosAsync() =>
        await ExportarEntidadesAsync(await _proyectoService.GetAllAsync(), "proyectos");

    private async Task ExportarTareasAsync() =>
        await ExportarEntidadesAsync(await _tareaService.GetAllAsync(), "tareas");

    private async Task ExportarEntidadesAsync<T>(IEnumerable<T> data, string nombre)
    {
        if (!CanExport) return;

        IsLoading = true;
        try
        {
            var path = await _exportService.ExportToCsvAsync(data, nombre);
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

    private List<ReporteExportRow> ConstruirFilasExportacion()
    {
        var filas = new List<ReporteExportRow>();
        filas.AddRange(EstadisticasEmpleados.Select(kv => new ReporteExportRow
        {
            Seccion = "Empleados",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        filas.AddRange(EstadisticasProyectos.Select(kv => new ReporteExportRow
        {
            Seccion = "Proyectos",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        filas.AddRange(EstadisticasTareas.Select(kv => new ReporteExportRow
        {
            Seccion = "Tareas",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        filas.AddRange(EstadisticasClientes.Select(kv => new ReporteExportRow
        {
            Seccion = "Clientes",
            Metrica = kv.Key,
            Valor = kv.Value
        }));
        return filas;
    }
}
