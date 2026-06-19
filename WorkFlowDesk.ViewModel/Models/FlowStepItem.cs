using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Paso visual del constructor de flujos.</summary>
public partial class FlowStepItem : ObservableObject
{
    private FlowStepType _tipo;
    private string _etiquetaCorta = string.Empty;
    private string _descripcion = string.Empty;
    private bool _mostrarConectorPrevio;

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public FlowStepType Tipo
    {
        get => _tipo;
        set
        {
            if (SetProperty(ref _tipo, value))
                NotificarApariencia();
        }
    }

    public string EtiquetaCorta
    {
        get => _etiquetaCorta;
        set => SetProperty(ref _etiquetaCorta, value);
    }

    public string Descripcion
    {
        get => _descripcion;
        set => SetProperty(ref _descripcion, value);
    }

    public bool MostrarConectorPrevio
    {
        get => _mostrarConectorPrevio;
        set => SetProperty(ref _mostrarConectorPrevio, value);
    }

    public string TipoLabel => FlowStepVisuals.Get(Tipo).Label;

    public string Icono => FlowStepVisuals.Get(Tipo).Icono;

    public string IconoColor => FlowStepVisuals.Get(Tipo).IconoColor;

    public string IconoFondo => FlowStepVisuals.Get(Tipo).IconoFondo;

    public string BordeCard => FlowStepVisuals.Get(Tipo).BordeCard;

    public string FondoCard => FlowStepVisuals.Get(Tipo).FondoCard;

    public string BordeCardGrueso => FlowStepVisuals.Get(Tipo).BordeCardGrueso;

    public bool EsTrigger => Tipo == FlowStepType.Trigger;

    public static FlowStepItem Crear(FlowStepType tipo, string etiqueta, string descripcion) =>
        new()
        {
            Tipo = tipo,
            EtiquetaCorta = etiqueta,
            Descripcion = descripcion
        };

    public FlowStepItem Clonar() =>
        new()
        {
            Id = Id,
            Tipo = Tipo,
            EtiquetaCorta = EtiquetaCorta,
            Descripcion = Descripcion,
            MostrarConectorPrevio = MostrarConectorPrevio
        };

    public void NotificarApariencia()
    {
        OnPropertyChanged(nameof(TipoLabel));
        OnPropertyChanged(nameof(Icono));
        OnPropertyChanged(nameof(IconoColor));
        OnPropertyChanged(nameof(IconoFondo));
        OnPropertyChanged(nameof(BordeCard));
        OnPropertyChanged(nameof(FondoCard));
        OnPropertyChanged(nameof(BordeCardGrueso));
        OnPropertyChanged(nameof(EsTrigger));
    }
}
