namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Resumen de tarea mostrado en el panel de proyecto.</summary>
public class TareaProyectoItem
{
    public string Titulo { get; init; } = string.Empty;
    public string EstadoTexto { get; init; } = string.Empty;
    public string PrioridadTexto { get; init; } = string.Empty;
}
