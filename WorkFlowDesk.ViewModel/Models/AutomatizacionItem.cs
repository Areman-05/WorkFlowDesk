using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Fila de automatización activa en la vista de optimización.</summary>
public partial class AutomatizacionItem : ObservableObject
{
    public string Id { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public string Trigger { get; init; } = string.Empty;
    public string UltimaEjecucion { get; init; } = string.Empty;
    public string Icono { get; init; } = string.Empty;
    public string IconoColor { get; init; } = "#9E00B5";

    [ObservableProperty]
    private bool _activa;
}
