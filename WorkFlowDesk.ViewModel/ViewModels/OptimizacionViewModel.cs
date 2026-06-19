using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de optimización de flujos y automatizaciones.</summary>
public partial class OptimizacionViewModel : ViewModelBase, ISearchableViewModel
{
    private readonly ITareaService _tareaService;
    private readonly IExportService _exportService;
    private string _textoBusqueda = string.Empty;
    private string _nombreFlujo = "Nuevo Proceso";
    private int _automatizacionesActivas;
    private string? _flujoEnEdicionId;

    public OptimizacionViewModel(ITareaService tareaService, IProyectoService proyectoService, IExportService exportService)
    {
        _tareaService = tareaService;
        _exportService = exportService;
        _ = proyectoService;

        Recomendaciones = new ObservableCollection<RecomendacionIaItem>();
        PasosFlujo = new ObservableCollection<FlowStepItem>();
        PasosFlujo.CollectionChanged += OnPasosFlujoChanged;
        Automatizaciones = new ObservableCollection<AutomatizacionItem>();
        AutomatizacionesFiltradas = new ObservableCollection<AutomatizacionItem>();

        ConfigurarRecomendacionCommand = new RelayCommand<RecomendacionIaItem>(ConfigurarRecomendacion);
        LimpiarLienzoCommand = new RelayCommand(LimpiarLienzo);
        GuardarFlujoCommand = new RelayCommand(GuardarFlujo);
        NuevoFlujoCommand = new RelayCommand(NuevoFlujo);
        ExportarFlujosCommand = new AsyncRelayCommand(ExportarFlujosAsync);
        AnadirPasoCommand = new RelayCommand(AnadirPaso);
        EditarPasoCommand = new RelayCommand<FlowStepItem>(EditarPaso);
        EditarAutomatizacionCommand = new RelayCommand<AutomatizacionItem>(EditarAutomatizacion);

        CargarDatosCommand = new AsyncRelayCommand(CargarDatosAsync);
        CargarDatosCommand.ExecuteAsync(null);
    }

    public ObservableCollection<RecomendacionIaItem> Recomendaciones { get; }
    public ObservableCollection<FlowStepItem> PasosFlujo { get; }

    public int ConteoPasos => PasosFlujo.Count;
    public ObservableCollection<AutomatizacionItem> Automatizaciones { get; }
    public ObservableCollection<AutomatizacionItem> AutomatizacionesFiltradas { get; }

    public string NombreFlujo
    {
        get => _nombreFlujo;
        set => SetProperty(ref _nombreFlujo, value);
    }

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
    public IAsyncRelayCommand ExportarFlujosCommand { get; }
    public IRelayCommand AnadirPasoCommand { get; }
    public IRelayCommand<FlowStepItem> EditarPasoCommand { get; }
    public IRelayCommand<AutomatizacionItem> EditarAutomatizacionCommand { get; }

    public event EventHandler<string>? AccionCompletada;
    public event EventHandler<FlowStepEditorRequest>? EditorPasoSolicitado;
    public event EventHandler<string>? ExportacionCompletada;

    public void AplicarPaso(FlowStepItem paso, bool esNuevo)
    {
        if (esNuevo)
        {
            PasosFlujo.Add(paso);
        }
        else
        {
            var existente = PasosFlujo.FirstOrDefault(p => p.Id == paso.Id);
            if (existente == null)
                return;

            existente.Tipo = paso.Tipo;
            existente.EtiquetaCorta = paso.EtiquetaCorta;
            existente.Descripcion = paso.Descripcion;
        }

        ActualizarConectores();
        PersistirBorrador();
    }

    public void EliminarPaso(string pasoId)
    {
        var paso = PasosFlujo.FirstOrDefault(p => p.Id == pasoId);
        if (paso == null)
            return;

        PasosFlujo.Remove(paso);
        ActualizarConectores();
        PersistirBorrador();
        AccionCompletada?.Invoke(this, "Paso eliminado del flujo.");
    }

    private async Task CargarDatosAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var tareas = (await _tareaService.GetAllAsync()).ToList();
            CargarRecomendaciones(tareas);
            CargarDesdePersistencia();
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

    private void CargarDesdePersistencia()
    {
        var data = FlujoWorkflowService.Load();
        NombreFlujo = data.NombreFlujoActual;
        _flujoEnEdicionId = null;

        PasosFlujo.Clear();
        foreach (var paso in data.PasosActuales.Select(MapPaso))
            PasosFlujo.Add(paso);
        ActualizarConectores();

        Automatizaciones.Clear();
        foreach (var auto in data.Automatizaciones)
        {
            var item = MapAutomatizacion(auto);
            item.PropertyChanged += OnAutomatizacionItemChanged;
            Automatizaciones.Add(item);
        }
    }

    private void OnAutomatizacionItemChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AutomatizacionItem.Activa))
        {
            ActualizarContadorActivas();
            PersistirAutomatizaciones();
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
            ResaltadoFondo = "#1AA45E86",
            ResaltadoBorde = "#66A45E86"
        });
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
        if (item == null)
            return;

        switch (item.Id)
        {
            case "asignacion-urgente":
                NombreFlujo = "Asignación Urgente";
                CargarPasos(
                    FlowStepItem.Crear(FlowStepType.Trigger, "Cada vez que...", "Una tarea crítica supera su fecha límite"),
                    FlowStepItem.Crear(FlowStepType.Condicion, "Si...", "Prioridad es \"Alta\" o \"Crítica\""),
                    FlowStepItem.Crear(FlowStepType.Accion, "Entonces...", "Reasignar al supervisor disponible"));
                break;
            case "recordatorios":
                NombreFlujo = "Recordatorios de Fechas";
                CargarPasos(
                    FlowStepItem.Crear(FlowStepType.Trigger, "Cada vez que...", "Una tarea tiene fecha de entrega"),
                    FlowStepItem.Crear(FlowStepType.Condicion, "Si...", "Faltan 48 horas para el vencimiento"),
                    FlowStepItem.Crear(FlowStepType.Accion, "Entonces...", "Enviar alerta al responsable"));
                break;
            case "reporte-auto":
                NombreFlujo = "Reporte Automático";
                CargarPasos(
                    FlowStepItem.Crear(FlowStepType.Trigger, "Cada viernes...", "Son las 17:00"),
                    FlowStepItem.Crear(FlowStepType.Condicion, "Si...", "Hay tareas o proyectos activos"),
                    FlowStepItem.Crear(FlowStepType.Accion, "Entonces...", "Generar resumen ejecutivo en PDF"));
                break;
        }

        _flujoEnEdicionId = null;
        PersistirBorrador();
        AccionCompletada?.Invoke(this, $"Plantilla «{item.Titulo}» cargada en el constructor.");
    }

    private void LimpiarLienzo()
    {
        if (!SolicitarConfirmacion(
                "¿Restablecer el constructor? Se perderán los pasos actuales del lienzo.",
                "Limpiar lienzo"))
            return;

        NombreFlujo = "Nuevo Proceso";
        _flujoEnEdicionId = null;
        PasosFlujo.Clear();
        PersistirBorrador();
        AccionCompletada?.Invoke(this, "Lienzo del constructor restablecido.");
    }

    private void GuardarFlujo()
    {
        if (PasosFlujo.Count == 0)
        {
            ErrorMessage = "Añade al menos un paso antes de guardar el flujo.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NombreFlujo))
        {
            ErrorMessage = "Indica un nombre para el flujo.";
            return;
        }

        ErrorMessage = null;
        var data = FlujoWorkflowService.Load();
        var id = _flujoEnEdicionId ?? $"flujo-{Guid.NewGuid():N}"[..20];
        var trigger = PasosFlujo.FirstOrDefault(p => p.Tipo == FlowStepType.Trigger);
        var autoData = new AutomatizacionData
        {
            Id = id,
            Nombre = NombreFlujo.Trim(),
            Descripcion = trigger != null
                ? $"{trigger.EtiquetaCorta} {trigger.Descripcion}"
                : "Flujo personalizado",
            Trigger = trigger != null ? $"{trigger.EtiquetaCorta} {trigger.Descripcion}" : "Personalizado",
            UltimaEjecucion = "Recién guardado",
            Icono = "\uE8A5",
            IconoColor = "#9E00B5",
            Activa = true,
            Pasos = PasosFlujo.Select(MapPasoData).ToList()
        };

        var existente = data.Automatizaciones.FindIndex(a => a.Id == id);
        if (existente >= 0)
            data.Automatizaciones[existente] = autoData;
        else
            data.Automatizaciones.Insert(0, autoData);

        data.NombreFlujoActual = NombreFlujo.Trim();
        data.PasosActuales = PasosFlujo.Select(MapPasoData).ToList();
        FlujoWorkflowService.Save(data);

        _flujoEnEdicionId = id;
        CargarDesdePersistencia();
        AccionCompletada?.Invoke(this, $"Flujo «{NombreFlujo}» guardado correctamente.");
    }

    private void NuevoFlujo()
    {
        if (PasosFlujo.Count > 0 &&
            !SolicitarConfirmacion(
                "¿Crear un flujo nuevo? Los cambios no guardados del lienzo actual se descartarán.",
                "Nuevo flujo"))
            return;

        NombreFlujo = "Nuevo Proceso";
        _flujoEnEdicionId = null;
        PasosFlujo.Clear();
        PersistirBorrador();
        AccionCompletada?.Invoke(this, "Nuevo flujo iniciado. Añade pasos para comenzar.");
    }

    private async Task ExportarFlujosAsync()
    {
        IsLoading = true;
        try
        {
            var filas = Automatizaciones.Select(a => new
            {
                a.Nombre,
                a.Descripcion,
                a.Trigger,
                Activa = a.Activa ? "Sí" : "No",
                a.UltimaEjecucion
            }).ToList();

            if (filas.Count == 0)
            {
                ErrorMessage = "No hay automatizaciones para exportar.";
                return;
            }

            var path = await _exportService.ExportToCsvAsync(filas, "automatizaciones");
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

    private void AnadirPaso()
    {
        var tipo = PasosFlujo.Count switch
        {
            0 => FlowStepType.Trigger,
            _ when !PasosFlujo.Any(p => p.Tipo == FlowStepType.Condicion) => FlowStepType.Condicion,
            _ => FlowStepType.Accion
        };

        EditorPasoSolicitado?.Invoke(this, new FlowStepEditorRequest(null, tipo));
    }

    private void EditarPaso(FlowStepItem? paso)
    {
        if (paso == null)
            return;

        EditorPasoSolicitado?.Invoke(this, new FlowStepEditorRequest(paso, paso.Tipo));
    }

    private void EditarAutomatizacion(AutomatizacionItem? item)
    {
        if (item == null)
            return;

        var data = FlujoWorkflowService.Load();
        var guardado = data.Automatizaciones.FirstOrDefault(a => a.Id == item.Id);
        NombreFlujo = item.Nombre;
        _flujoEnEdicionId = item.Id;

        PasosFlujo.Clear();
        if (guardado?.Pasos.Count > 0)
        {
            foreach (var paso in guardado.Pasos.Select(MapPaso))
                PasosFlujo.Add(paso);
        }
        else
        {
            CargarPasos(
                FlowStepItem.Crear(FlowStepType.Trigger, "Cada vez que...", item.Trigger),
                FlowStepItem.Crear(FlowStepType.Accion, "Entonces...", item.Descripcion));
        }

        ActualizarConectores();
        PersistirBorrador();
        AccionCompletada?.Invoke(this, $"Flujo «{item.Nombre}» cargado para editar.");
    }

    private void CargarPasos(params FlowStepItem[] pasos)
    {
        PasosFlujo.Clear();
        foreach (var paso in pasos)
            PasosFlujo.Add(paso);
        ActualizarConectores();
    }

    private void OnPasosFlujoChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ConteoPasos));
    }

    private void ActualizarConectores()
    {
        for (var i = 0; i < PasosFlujo.Count; i++)
            PasosFlujo[i].MostrarConectorPrevio = i > 0;
    }

    private void PersistirBorrador()
    {
        var data = FlujoWorkflowService.Load();
        data.NombreFlujoActual = NombreFlujo;
        data.PasosActuales = PasosFlujo.Select(MapPasoData).ToList();
        FlujoWorkflowService.Save(data);
    }

    private void PersistirAutomatizaciones()
    {
        var data = FlujoWorkflowService.Load();
        foreach (var item in Automatizaciones)
        {
            var auto = data.Automatizaciones.FirstOrDefault(a => a.Id == item.Id);
            if (auto != null)
                auto.Activa = item.Activa;
        }

        FlujoWorkflowService.Save(data);
    }

    private static FlowStepItem MapPaso(FlowStepData data)
    {
        var item = FlowStepItem.Crear(
            Enum.TryParse<FlowStepType>(data.Tipo, out var tipo) ? tipo : FlowStepType.Trigger,
            data.EtiquetaCorta,
            data.Descripcion);
        item.Id = data.Id;
        return item;
    }

    private static FlowStepData MapPasoData(FlowStepItem paso) => new()
    {
        Id = paso.Id,
        Tipo = paso.Tipo.ToString(),
        EtiquetaCorta = paso.EtiquetaCorta,
        Descripcion = paso.Descripcion
    };

    private static AutomatizacionItem MapAutomatizacion(AutomatizacionData data) => new()
    {
        Id = data.Id,
        Nombre = data.Nombre,
        Descripcion = data.Descripcion,
        Trigger = data.Trigger,
        UltimaEjecucion = data.UltimaEjecucion,
        Icono = data.Icono,
        IconoColor = data.IconoColor,
        Activa = data.Activa
    };
}

public sealed class FlowStepEditorRequest
{
    public FlowStepEditorRequest(FlowStepItem? pasoExistente, FlowStepType tipoPorDefecto)
    {
        PasoExistente = pasoExistente;
        TipoPorDefecto = tipoPorDefecto;
    }

    public FlowStepItem? PasoExistente { get; }
    public FlowStepType TipoPorDefecto { get; }
    public bool EsNuevo => PasoExistente == null;
}
