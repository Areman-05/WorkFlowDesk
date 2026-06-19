namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Tarjeta de recomendación de IA en la vista de optimización.</summary>
public class RecomendacionIaItem
{
    public string Id { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public string Icono { get; init; } = string.Empty;
    public string IconoColor { get; init; } = "#9E00B5";
    public string IconoFondo { get; init; } = "#1A9E00B5";
    public string BordeColor { get; init; } = "#FD933D";
    public string ResaltadoFondo { get; init; } = "#1AFD933D";
    public string ResaltadoBorde { get; init; } = "#4DFD933D";
    public bool ResaltadoFucsia { get; init; }
}
