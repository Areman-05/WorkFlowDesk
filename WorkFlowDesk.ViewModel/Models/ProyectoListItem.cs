using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Fila de listado de proyectos para la vista Stitch.</summary>
public class ProyectoListItem
{
    public required Proyecto Proyecto { get; init; }
    public string Iniciales { get; init; } = "PR";
    public string CodigoId { get; init; } = string.Empty;
    public string NombreCliente { get; init; } = "—";
    public string FechaInicioTexto { get; init; } = string.Empty;
    public string EstadoTexto { get; init; } = string.Empty;
    public string EstadoFondo { get; init; } = "#E0E7FF";
    public string EstadoTextoColor { get; init; } = "#3730A3";
    public int Progreso { get; init; }
    public bool ProgresoCompleto { get; init; }
}
