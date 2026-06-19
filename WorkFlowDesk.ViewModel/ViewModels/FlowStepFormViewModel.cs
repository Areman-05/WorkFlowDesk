using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>Formulario modal para crear o editar un paso del flujo.</summary>
public class FlowStepFormViewModel : ViewModelBase
{
    private FlowStepType _tipo;
    private string _etiquetaCorta = string.Empty;
    private string _descripcion = string.Empty;
    private readonly bool _esEdicion;
    private readonly string _pasoId;

    public FlowStepFormViewModel(FlowStepItem? pasoExistente = null, FlowStepType? tipoPorDefecto = null)
    {
        _esEdicion = pasoExistente != null;
        _pasoId = pasoExistente?.Id ?? Guid.NewGuid().ToString();
        _tipo = pasoExistente?.Tipo ?? tipoPorDefecto ?? FlowStepType.Trigger;
        _etiquetaCorta = pasoExistente?.EtiquetaCorta ?? ObtenerEtiquetaPorDefecto(_tipo);
        _descripcion = pasoExistente?.Descripcion ?? ObtenerDescripcionPorDefecto(_tipo);

        TiposDisponibles = new ObservableCollection<TipoPasoOption>(
        [
            new TipoPasoOption { Tipo = FlowStepType.Trigger, Nombre = "Trigger (disparador)" },
            new TipoPasoOption { Tipo = FlowStepType.Condicion, Nombre = "Condición" },
            new TipoPasoOption { Tipo = FlowStepType.Accion, Nombre = "Acción" }
        ]);

        GuardarCommand = new RelayCommand(Guardar, CanGuardar);
        CancelarCommand = new RelayCommand(Cancelar);
        EliminarCommand = new RelayCommand(Eliminar);
    }

    public string Titulo => _esEdicion ? "Editar paso" : "Añadir paso";

    public string Subtitulo => _esEdicion
        ? "Modifica el trigger, condición o acción del flujo"
        : "Define un nuevo paso en el constructor de flujos";

    public bool PuedeEliminar => _esEdicion;

    public ObservableCollection<TipoPasoOption> TiposDisponibles { get; }

    public TipoPasoOption TipoSeleccionado
    {
        get => TiposDisponibles.First(t => t.Tipo == _tipo);
        set
        {
            if (value == null || _tipo == value.Tipo)
                return;

            _tipo = value.Tipo;
            OnPropertyChanged(nameof(TipoSeleccionado));
            OnPropertyChanged(nameof(EtiquetaAyuda));
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string EtiquetaCorta
    {
        get => _etiquetaCorta;
        set
        {
            if (SetProperty(ref _etiquetaCorta, value))
                GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        set
        {
            if (SetProperty(ref _descripcion, value))
                GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string EtiquetaAyuda => _tipo switch
    {
        FlowStepType.Trigger => "Ej.: Cada vez que..., Al crear..., Cuando cambie...",
        FlowStepType.Condicion => "Ej.: Si..., Cuando..., Mientras...",
        FlowStepType.Accion => "Ej.: Entonces..., Ejecutar..., Notificar...",
        _ => string.Empty
    };

    public IRelayCommand GuardarCommand { get; }
    public IRelayCommand CancelarCommand { get; }
    public IRelayCommand EliminarCommand { get; }

    public event EventHandler<FlowStepItem>? Guardado;
    public event EventHandler<string>? Eliminado;
    public event EventHandler? Cancelado;

    private void Guardar()
    {
        var paso = new FlowStepItem
        {
            Id = _pasoId,
            Tipo = _tipo,
            EtiquetaCorta = EtiquetaCorta.Trim(),
            Descripcion = Descripcion.Trim()
        };
        Guardado?.Invoke(this, paso);
    }

    private bool CanGuardar() =>
        !string.IsNullOrWhiteSpace(EtiquetaCorta) &&
        !string.IsNullOrWhiteSpace(Descripcion);

    private void Cancelar() => Cancelado?.Invoke(this, EventArgs.Empty);

    private void Eliminar() => Eliminado?.Invoke(this, _pasoId);

    private static string ObtenerEtiquetaPorDefecto(FlowStepType tipo) => tipo switch
    {
        FlowStepType.Trigger => "Cada vez que...",
        FlowStepType.Condicion => "Si...",
        FlowStepType.Accion => "Entonces...",
        _ => "Paso..."
    };

    private static string ObtenerDescripcionPorDefecto(FlowStepType tipo) => tipo switch
    {
        FlowStepType.Trigger => "Se crea una nueva tarea",
        FlowStepType.Condicion => "Prioridad es \"Alta\"",
        FlowStepType.Accion => "Asignar a PM Senior",
        _ => string.Empty
    };

    public sealed class TipoPasoOption
    {
        public FlowStepType Tipo { get; init; }
        public string Nombre { get; init; } = string.Empty;
    }
}
