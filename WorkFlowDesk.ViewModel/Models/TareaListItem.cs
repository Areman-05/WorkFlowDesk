using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Fila de listado de tareas para la vista Stitch.</summary>
public class TareaListItem
{
    public required Tarea Tarea { get; init; }
    public string PrioridadTexto { get; init; } = string.Empty;
    public string PrioridadFondo { get; init; } = "#FEE2E2";
    public string PrioridadTextoColor { get; init; } = "#B91C1C";
    public string EstadoTexto { get; init; } = string.Empty;
    public string EstadoFondo { get; init; } = "#DBEAFE";
    public string EstadoTextoColor { get; init; } = "#1D4ED8";
    public string NombreProyecto { get; init; } = "—";
    public string AsignadoNombre { get; init; } = "Sin asignar";
    public string AsignadoIniciales { get; init; } = "?";
    public int AvatarIndex { get; init; }
    public string FechaLimiteTexto { get; init; } = "—";
    public bool TituloTachado { get; init; }
}
