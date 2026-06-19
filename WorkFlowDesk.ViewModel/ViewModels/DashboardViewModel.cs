using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel del panel principal con estadísticas.</summary>
public class DashboardViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IProyectoService _proyectoService;
    private readonly ITareaService _tareaService;
    private readonly IClienteService _clienteService;

    private int _totalEmpleados;
    private int _totalProyectos;
    private int _totalTareas;
    private int _totalClientes;
    private int _tareasPendientes;
    private int _proyectosEnProgreso;
    private int _tareasUrgentes;
    private int _tareasEstaSemana;
    private int _tareasSinFecha;
    private int _porcentajePendientes;
    private bool _tieneProyectosActivos;
    private bool _mostrarStatsAdmin;
    private string _badgeEmpleados = string.Empty;
    private string _badgeProyectos = string.Empty;
    private string _badgeTareas = string.Empty;
    private string _badgeClientes = string.Empty;
    private string _subtituloBienvenida = string.Empty;

    public DashboardViewModel(
        IEmpleadoService empleadoService,
        IProyectoService proyectoService,
        ITareaService tareaService,
        IClienteService clienteService)
    {
        _empleadoService = empleadoService;
        _proyectoService = proyectoService;
        _tareaService = tareaService;
        _clienteService = clienteService;

        ProyectosActivos = new ObservableCollection<DashboardProyectoItem>();
        ProyectosPrioritarios = new ObservableCollection<DashboardProyectoPrioritarioItem>();
        ActividadesRecientes = new ObservableCollection<DashboardActividadItem>();

        CargarEstadisticasCommand = new AsyncRelayCommand(CargarEstadisticasAsync);
        VerTareasCommand = new RelayCommand(() => AppNavigationService.RequestSection("Tareas"));
        VerProyectosCommand = new RelayCommand(() => AppNavigationService.RequestSection("Proyectos"));
        IrOptimizacionCommand = new RelayCommand(() => AppNavigationService.RequestSection("Optimizacion"));
        EditarProyectoCommand = new RelayCommand<DashboardProyectoPrioritarioItem>(EditarProyecto);

        CargarEstadisticasCommand.ExecuteAsync(null);
    }

    public event EventHandler<int>? ProyectoEdicionSolicitado;

    public ObservableCollection<DashboardProyectoItem> ProyectosActivos { get; }
    public ObservableCollection<DashboardProyectoPrioritarioItem> ProyectosPrioritarios { get; }
    public ObservableCollection<DashboardActividadItem> ActividadesRecientes { get; }

    public string SubtituloBienvenida
    {
        get => _subtituloBienvenida;
        private set => SetProperty(ref _subtituloBienvenida, value);
    }

    public bool MostrarStatsAdmin
    {
        get => _mostrarStatsAdmin;
        private set => SetProperty(ref _mostrarStatsAdmin, value);
    }

    public int TotalEmpleados { get => _totalEmpleados; set => SetProperty(ref _totalEmpleados, value); }
    public int TotalProyectos { get => _totalProyectos; set => SetProperty(ref _totalProyectos, value); }
    public int TotalTareas { get => _totalTareas; set => SetProperty(ref _totalTareas, value); }
    public int TotalClientes { get => _totalClientes; set => SetProperty(ref _totalClientes, value); }
    public int TareasPendientes { get => _tareasPendientes; set => SetProperty(ref _tareasPendientes, value); }
    public int ProyectosEnProgreso { get => _proyectosEnProgreso; set => SetProperty(ref _proyectosEnProgreso, value); }
    public int TareasUrgentes { get => _tareasUrgentes; set => SetProperty(ref _tareasUrgentes, value); }
    public int TareasEstaSemana { get => _tareasEstaSemana; set => SetProperty(ref _tareasEstaSemana, value); }
    public int TareasSinFecha { get => _tareasSinFecha; set => SetProperty(ref _tareasSinFecha, value); }
    public int PorcentajePendientes { get => _porcentajePendientes; set => SetProperty(ref _porcentajePendientes, value); }
    public bool TieneProyectosActivos { get => _tieneProyectosActivos; set => SetProperty(ref _tieneProyectosActivos, value); }

    public string BadgeEmpleados { get => _badgeEmpleados; set => SetProperty(ref _badgeEmpleados, value); }
    public string BadgeProyectos { get => _badgeProyectos; set => SetProperty(ref _badgeProyectos, value); }
    public string BadgeTareas { get => _badgeTareas; set => SetProperty(ref _badgeTareas, value); }
    public string BadgeClientes { get => _badgeClientes; set => SetProperty(ref _badgeClientes, value); }

    public IAsyncRelayCommand CargarEstadisticasCommand { get; }
    public IRelayCommand VerTareasCommand { get; }
    public IRelayCommand VerProyectosCommand { get; }
    public IRelayCommand IrOptimizacionCommand { get; }
    public IRelayCommand<DashboardProyectoPrioritarioItem> EditarProyectoCommand { get; }

    public event EventHandler? DatosActualizados;

    private void EditarProyecto(DashboardProyectoPrioritarioItem? proyecto)
    {
        if (proyecto != null)
            ProyectoEdicionSolicitado?.Invoke(this, proyecto.Id);
    }

    private bool _cargaInicialCompleta;

    private async Task CargarEstadisticasAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        var exito = false;

        try
        {
            var esAdmin = SessionService.IsAdmin();
            var esSupervisor = SessionService.IsSupervisor() && !esAdmin;
            var esEmpleado = RolePermissions.IsReadOnlyUser;

            MostrarStatsAdmin = !esEmpleado;
            SubtituloBienvenida = ConstruirSubtitulo(esAdmin, esSupervisor);

            var empleadoActual = await ObtenerEmpleadoActualAsync();
            var empleados = (await _empleadoService.GetAllAsync()).ToList();
            var proyectos = (await _proyectoService.GetAllAsync()).ToList();
            var tareas = (await _tareaService.GetAllAsync()).ToList();
            var clientes = (await _clienteService.GetAllAsync()).ToList();

            if (esEmpleado && empleadoActual != null)
            {
                tareas = tareas.Where(t => t.AsignadoId == empleadoActual.Id).ToList();
                var proyectoIds = tareas.Select(t => t.ProyectoId).Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToHashSet();
                proyectos = proyectos.Where(p => proyectoIds.Contains(p.Id)).ToList();
            }
            else if (esSupervisor && empleadoActual != null)
            {
                proyectos = proyectos.Where(p => p.ResponsableId == empleadoActual.Id).ToList();
                var proyectoIds = proyectos.Select(p => p.Id).ToHashSet();
                tareas = tareas.Where(t => t.ProyectoId.HasValue && proyectoIds.Contains(t.ProyectoId.Value)).ToList();
            }

            TotalEmpleados = esEmpleado ? 0 : empleados.Count;
            TotalProyectos = proyectos.Count;
            TotalTareas = tareas.Count;
            TotalClientes = esEmpleado ? 0 : clientes.Count;

            var pendientes = tareas.Where(t => t.Estado == EstadoTarea.Pendiente).ToList();
            TareasPendientes = pendientes.Count;
            TareasUrgentes = tareas.Count(t =>
                (t.Estado == EstadoTarea.Pendiente || t.Estado == EstadoTarea.EnProgreso) &&
                (t.Prioridad == PrioridadTarea.Alta || t.Prioridad == PrioridadTarea.Critica));
            TareasSinFecha = pendientes.Count(t => !t.FechaVencimiento.HasValue);

            var inicioSemana = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek + (DateTime.Now.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
            TareasEstaSemana = tareas.Count(t =>
                t.FechaVencimiento.HasValue && t.FechaVencimiento.Value.Date >= inicioSemana &&
                t.FechaVencimiento.Value.Date <= inicioSemana.AddDays(6) &&
                t.Estado != EstadoTarea.Completada && t.Estado != EstadoTarea.Cancelada);

            PorcentajePendientes = TotalTareas > 0 ? (int)Math.Round(100.0 * TareasPendientes / TotalTareas) : 0;

            var enProgreso = proyectos.Where(p => p.Estado == EstadoProyecto.EnProgreso).ToList();
            ProyectosEnProgreso = enProgreso.Count;

            ActualizarBadges(empleados, proyectos, tareas, clientes, esEmpleado);
            ActualizarProyectosActivos(enProgreso, tareas);
            ActualizarProyectosPrioritarios(proyectos);
            ActualizarActividad(tareas, clientes, empleados);

            TieneProyectosActivos = ProyectosActivos.Count > 0;
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

    private static string ConstruirSubtitulo(bool esAdmin, bool esSupervisor)
    {
        var nombre = SessionService.GetUserName();
        var rol = esAdmin ? "Administrador" : esSupervisor ? "Supervisor" : "Empleado";
        return $"Bienvenido de nuevo, {rol}. Aquí tienes el resumen de hoy.";
    }

    private async Task<Empleado?> ObtenerEmpleadoActualAsync()
    {
        var userId = SessionService.CurrentUser?.Id;
        if (userId == null) return null;
        var empleados = await _empleadoService.GetAllAsync();
        return empleados.FirstOrDefault(e => e.UsuarioId == userId);
    }

    private void ActualizarBadges(
        List<Empleado> empleados, List<Proyecto> proyectos, List<Tarea> tareas, List<Cliente> clientes, bool esEmpleado)
    {
        if (esEmpleado)
        {
            BadgeEmpleados = BadgeClientes = string.Empty;
            BadgeProyectos = proyectos.Any(p => p.Estado == EstadoProyecto.EnProgreso) ? "Activos" : "—";
            var nuevas = tareas.Count(t => t.FechaCreacion >= DateTime.Now.AddDays(-7));
            BadgeTareas = nuevas > 0 ? $"{nuevas} nuevas" : "Al día";
            return;
        }

        var empleadosRecientes = empleados.Count(e => e.FechaContratacion >= DateTime.Now.AddMonths(-1));
        BadgeEmpleados = empleadosRecientes > 0 ? $"+{empleadosRecientes} este mes" : "Estable";

        BadgeProyectos = proyectos.Any(p => p.Estado == EstadoProyecto.EnProgreso) ? "Activo" : "—";

        var tareasNuevas = tareas.Count(t => t.FechaCreacion >= DateTime.Now.AddDays(-7));
        BadgeTareas = tareasNuevas > 0 ? $"{tareasNuevas} nuevas" : "Al día";

        var clientesMes = clientes.Count(c => c.FechaCreacion >= DateTime.Now.AddMonths(-1));
        BadgeClientes = clientesMes > 0 ? $"+{clientesMes} este mes" : "Estable";
    }

    private void ActualizarProyectosActivos(List<Proyecto> enProgreso, List<Tarea> tareas)
    {
        ProyectosActivos.Clear();
        var colores = new[] { "#9E00B5", "#944A00", "#9E00B5", "#87466D" };
        var i = 0;
        foreach (var proyecto in enProgreso.Take(4))
        {
            var tareasProyecto = tareas.Where(t => t.ProyectoId == proyecto.Id).ToList();
            var total = tareasProyecto.Count;
            var completadas = tareasProyecto.Count(t => t.Estado == EstadoTarea.Completada);
            var pct = total > 0 ? (int)Math.Round(100.0 * completadas / total) : 0;

            ProyectosActivos.Add(new DashboardProyectoItem
            {
                Nombre = proyecto.Nombre,
                ProgresoPorcentaje = pct,
                BarColor = colores[i % colores.Length]
            });
            i++;
        }
    }

    private void ActualizarProyectosPrioritarios(List<Proyecto> proyectos)
    {
        ProyectosPrioritarios.Clear();
        foreach (var p in proyectos
                     .OrderByDescending(x => x.Estado == EstadoProyecto.EnProgreso)
                     .ThenByDescending(x => x.FechaCreacion)
                     .Take(5))
        {
            var (texto, fondo, color) = EstadoProyectoVisual(p.Estado);
            ProyectosPrioritarios.Add(new DashboardProyectoPrioritarioItem
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Subtitulo = string.IsNullOrWhiteSpace(p.Descripcion) ? "—" : p.Descripcion,
                Cliente = p.Cliente?.Empresa ?? p.Cliente?.Nombre ?? "Interno",
                EstadoTexto = texto,
                EstadoFondo = fondo,
                EstadoTextoColor = color,
                Inicial = string.IsNullOrEmpty(p.Nombre) ? "?" : p.Nombre[0].ToString().ToUpperInvariant(),
                AvatarFondo = p.Estado == EstadoProyecto.Completado ? "#DCFCE7" :
                    p.Estado == EstadoProyecto.Planificacion ? "#FFEBCC" : "#FBABFF"
            });
        }
    }

    private void ActualizarActividad(List<Tarea> tareas, List<Cliente> clientes, List<Empleado> empleados)
    {
        ActividadesRecientes.Clear();
        var eventos = new List<DashboardActividadItem>();

        foreach (var t in tareas.OrderByDescending(t => t.FechaCreacion).Take(5))
        {
            var creador = empleados.FirstOrDefault(e => e.Id == t.CreadorId);
            var nombre = creador != null ? $"{creador.Nombre} {creador.Apellidos}" : "Alguien";
            var proyecto = t.Proyecto?.Nombre ?? "un proyecto";
            eventos.Add(new DashboardActividadItem
            {
                Icono = "\uE710",
                IconoFondo = "#1A9E00B5",
                IconoColor = "#9E00B5",
                Texto = $"{nombre} añadió la tarea «{t.Titulo}» en {proyecto}",
                TiempoRelativo = FormatearTiempoRelativo(t.FechaCreacion),
                Fecha = t.FechaCreacion
            });
        }

        foreach (var t in tareas.Where(t => t.Estado == EstadoTarea.Completada && t.FechaFin.HasValue)
                     .OrderByDescending(t => t.FechaFin).Take(3))
        {
            var asignado = empleados.FirstOrDefault(e => e.Id == t.AsignadoId);
            var nombre = asignado != null ? $"{asignado.Nombre} {asignado.Apellidos}" : "Alguien";
            eventos.Add(new DashboardActividadItem
            {
                Icono = "\uE73E",
                IconoFondo = "#1A944A00",
                IconoColor = "#944A00",
                Texto = $"{nombre} completó «{t.Titulo}»",
                TiempoRelativo = FormatearTiempoRelativo(t.FechaFin!.Value),
                Fecha = t.FechaFin!.Value
            });
        }

        foreach (var c in clientes.OrderByDescending(c => c.FechaCreacion).Take(2))
        {
            eventos.Add(new DashboardActividadItem
            {
                Icono = "\uE902",
                IconoFondo = "#1A87466D",
                IconoColor = "#6F3157",
                Texto = $"Nuevo cliente registrado: {c.Empresa}",
                TiempoRelativo = FormatearTiempoRelativo(c.FechaCreacion),
                Fecha = c.FechaCreacion
            });
        }

        foreach (var item in eventos.OrderByDescending(e => e.Fecha).Take(3))
            ActividadesRecientes.Add(item);
    }

    private static (string Texto, string Fondo, string Color) EstadoProyectoVisual(EstadoProyecto estado) =>
        estado switch
        {
            EstadoProyecto.EnProgreso => ("En progreso", "#DBEAFE", "#1D4ED8"),
            EstadoProyecto.Completado => ("Completada", "#DCFCE7", "#15803D"),
            EstadoProyecto.Planificacion => ("Pendiente", "#FEF9C3", "#A16207"),
            EstadoProyecto.EnPausa => ("En pausa", "#F3F4F6", "#4B5563"),
            EstadoProyecto.Cancelado => ("Cancelado", "#FEE2E2", "#B91C1C"),
            _ => ("—", "#E3DFFF", "#181445")
        };

    private static string FormatearTiempoRelativo(DateTime fecha)
    {
        var diff = DateTime.Now - fecha;
        if (diff.TotalMinutes < 60) return $"Hace {(int)Math.Max(1, diff.TotalMinutes)} minutos";
        if (diff.TotalHours < 24) return $"Hace {(int)diff.TotalHours} horas";
        if (diff.TotalDays < 7) return $"Hace {(int)diff.TotalDays} días";
        return fecha.ToString("dd MMM yyyy");
    }
}
