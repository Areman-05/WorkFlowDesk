namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Colores e iconos por tipo de paso.</summary>
public static class FlowStepVisuals
{
    public sealed record Visual(
        string Label,
        string Icono,
        string IconoColor,
        string IconoFondo,
        string BordeCard,
        string FondoCard,
        string BordeCardGrueso);

    public static Visual Get(FlowStepType tipo) => tipo switch
    {
        FlowStepType.Trigger => new Visual(
            "TRIGGER", "\uE823", "#9E00B5", "#1A9E00B5",
            "#9E00B5", "#CCFFFFFF", "#9E00B5"),
        FlowStepType.Condicion => new Visual(
            "CONDICIÓN", "\uE71C", "#944A00", "#1AFD933D",
            "#4DFD933D", "#1AFD933D", "#4DFD933D"),
        FlowStepType.Accion => new Visual(
            "ACCIÓN", "\uE77B", "#87466D", "#1AA45E86",
            "#A45E86", "#CCFFFFFF", "#A45E86"),
        _ => new Visual("PASO", "\uE710", "#9E00B5", "#1A9E00B5",
            "#9E00B5", "#CCFFFFFF", "#9E00B5")
    };
}
