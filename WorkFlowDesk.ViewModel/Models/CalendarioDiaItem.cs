namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Celda del calendario mensual de tareas.</summary>
public class CalendarioDiaItem
{
    public int Numero { get; init; }
    public bool EsMesActual { get; init; }
    public bool EsHoy { get; init; }
    public IReadOnlyList<TareaListItem> Tareas { get; init; } = Array.Empty<TareaListItem>();
}
