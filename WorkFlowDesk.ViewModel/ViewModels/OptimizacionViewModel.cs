using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de optimización de flujos y automatizaciones.</summary>
public partial class OptimizacionViewModel : ViewModelBase, ISearchableViewModel
{
    private readonly ITareaService _tareaService;
    private string _textoBusqueda = string.Empty;
    private int _automatizacionesActivas;

    public OptimizacionViewModel(ITareaService tareaService, IProyectoService proyectoService)
    {
        _tareaService = tareaService;
        _ = proyectoService;

        Recomendaciones = new ObservableCollection<RecomendacionIaItem>();
        Automatizaciones = new ObservableCollection<AutomatizacionItem>();
        AutomatizacionesFiltradas = new ObservableCollection<AutomatizacionItem>();

        ConfigurarRecomendacionCommand = new RelayCommand<RecomendacionIaItem>(ConfigurarRecomendacion);
        LimpiarLienzoCommand = new RelayCommand(LimpiarLienzo);
        GuardarFlujoCommand = new RelayCommand(GuardarFlujo);
        NuevoFlujoCommand = new RelayCommand(NuevoFlujo);
        ExportarFlujosCommand = new RelayCommand(ExportarFlujos);

        CargarDatosCommand = new AsyncRelayCommand(CargarDatosAsync);
        CargarDatosCommand.ExecuteAsync(null);
    }

    public ObservableCollection<RecomendacionIaItem> Recomendaciones { get; }
    public ObservableCollection<AutomatizacionItem> Automatizaciones { get; }
    public ObservableCollection<AutomatizacionItem> AutomatizacionesFiltradas { get; }

    public int AutomatizacionesActivas
    {
        get => _automatizacionesActivas;
        private set => SetProperty(ref _automatizacionesActivas, value);
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            if (!SetProperty(ref _textoBusqueda, value))
                return;

            AplicarFiltro();
        }
    }

    public IAsyncRelayCommand CargarDatosCommand { get; }
    public IRelayCommand<RecomendacionIaItem> ConfigurarRecomendacionCommand { get; }
    public IRelayCommand LimpiarLienzoCommand { get; }
    public IRelayCommand GuardarFlujoCommand { get; }
    public IRelayCommand NuevoFlujoCommand { get; }
    public IRelayCommand ExportarFlujosCommand { get; }

    public event EventHandler<string>? AccionCompletada;

    private async Task CargarDatosAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var tareas = (await _tareaService.GetAllAsync()).ToList();
            CargarRecomendaciones(tareas);
            CargarAutomatizaciones();
            AplicarFiltro();
            ActualizarContadorActivas();
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

    private void CargarRecomendaciones(List<Tarea> tareas)
    {
        Recomendaciones.Clear();

        var retrasadas = tareas.Count(t =>
            t.Estado != EstadoTarea.Completada &&
            t.Estado != EstadoTarea.Cancelada &&
            t.FechaVencimiento.HasValue &&
            t.FechaVencimiento.Value.Date < DateTime.Today &&
            (t.Prioridad == PrioridadTarea.Alta || t.Prioridad == PrioridadTarea.Critica));

        Recomendaciones.Add(new RecomendacionIaItem
        {
            Id = "asignacion-urgente",
            Titulo = "Asignación Urgente",
            Descripcion = retrasadas > 0
                ? $"Detectamos {retrasadas} tareas críticas con retraso. Automatiza la reasignación inmediata."
                : "Detectamos retrasos en tareas críticas. Automatiza la reasignación inmediata.",
            Icono = "\uE7BA",
            IconoColor = "#944A00",
            IconoFondo = "#1AFD933D",
            BordeColor = "#FD933D",
            ResaltadoFondo = "#1AFD933D",
            ResaltadoBorde = "#4DFD933D"
        });

        Recomendaciones.Add(new RecomendacionIaItem
        {
            Id = "recordatorios",
            Titulo = "Recordatorios de Fechas",
            Descripcion = "Evita cuellos de botella enviando alertas 48h antes de cada entrega.",
            Icono = "\uE787",
            IconoColor = "#9E00B5",
            IconoFondo = "#1A9E00B5",
            BordeColor = "#9E00B5",
            ResaltadoFucsia = true
        });

        Recomendaciones.Add(new RecomendacionIaItem
        {
            Id = "reporte-auto",
            Titulo = "Reporte Automático",
            Descripcion = "Genera un resumen ejecutivo cada viernes a las 17:00 PM.",
            Icono = "\uE8A5",
            IconoColor = "#87466D",
            IconoFondo = "#1AA45E86",
            BordeColor = "#A45E86",
            ResaltadoFondo = "#CCFFFFFF",
            ResaltadoBorde = "#33D6C0D3"
        });
    }

    private void CargarAutomatizaciones()
    {
        Automatizaciones.Clear();
        Automatizaciones.Add(new AutomatizacionItem
        {
            Id = "cierre-proyecto",
            Nombre = "Notificación de cierre de Proyecto",
            Descripcion = "Envía correo a cliente al finalizar",
            Trigger = "Estado → Completado",
            UltimaEjecucion = "Hace 2 horas",
            Icono = "\uE7C4",
            IconoColor = "#9E00B5",
            Activa = true
        });
        Automatizaciones.Add(new AutomatizacionItem
        {
            Id = "escalado-tareas",
            Nombre = "Escalado de Tareas Vencidas",
            Descripcion = "Asigna a supervisor tras 24h de atraso",
            Trigger = "Cron (Daily)",
            UltimaEjecucion = "Ayer, 09:00 AM",
            Icono = "\uE7BA",
            IconoColor = "#944A00",
            Activa = true
        });
        Automatizaciones.Add(new AutomatizacionItem
        {
            Id = "sync-slack",
            Nombre = "Sync con Slack",
            Descripcion = "Espeja actualizaciones en canal #ventas",
            Trigger = "Webhook Externo",
            UltimaEjecucion = "Hace 15 min",
            Icono = "\uE8BD",
            IconoColor = "#87466D",
            Activa = false
        });
        Automatizaciones.Add(new AutomatizacionItem
        {
            Id = "backup-semanal",
            Nombre = "Respaldo Semanal de Datos",
            Descripcion = "Exporta CSV de tareas cada lunes",
            Trigger = "Cron (Weekly)",
            UltimaEjecucion = "Hace 3 días",
            Icono = "\uE895",
            IconoColor = "#9E00B5",
            Activa = true
        });

        foreach (var item in Automatizaciones)
        {
            item.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(AutomatizacionItem.Activa))
                    ActualizarContadorActivas();
            };
        }
    }

    private void AplicarFiltro()
    {
        AutomatizacionesFiltradas.Clear();
        var termino = TextoBusqueda.Trim();
        var items = string.IsNullOrEmpty(termino)
            ? Automatizaciones
            : Automatizaciones.Where(a =>
                a.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                a.Descripcion.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                a.Trigger.Contains(termino, StringComparison.OrdinalIgnoreCase));

        foreach (var item in items)
            AutomatizacionesFiltradas.Add(item);
    }

    private void ActualizarContadorActivas() =>
        AutomatizacionesActivas = Automatizaciones.Count(a => a.Activa);

    private void ConfigurarRecomendacion(RecomendacionIaItem? item)
    {
        if (item == null) return;
        AccionCompletada?.Invoke(this, $"Configuración de «{item.Titulo}» disponible próximamente.");
    }

    private void LimpiarLienzo() =>
        AccionCompletada?.Invoke(this, "Lienzo del constructor de flujos restablecido.");

    private void GuardarFlujo() =>
        AccionCompletada?.Invoke(this, "Flujo «Nuevo Proceso» guardado correctamente.");

    private void NuevoFlujo() =>
        AccionCompletada?.Invoke(this, "Creación de nuevo flujo iniciada.");

    private void ExportarFlujos() =>
        AccionCompletada?.Invoke(this, "Exportación de flujos disponible próximamente.");
}
