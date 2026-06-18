using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Fila de listado de empleados para la vista Stitch.</summary>
public class EmpleadoListItem
{
    public required Empleado Empleado { get; init; }
    public int AvatarIndex { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string CodigoId { get; init; } = string.Empty;
    public string NombreUsuario { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Cargo { get; init; } = string.Empty;
    public string EstadoTexto { get; init; } = string.Empty;
    public string EstadoFondo { get; init; } = "#E6FCF5";
    public string EstadoTextoColor { get; init; } = "#0CA678";
    public string Iniciales { get; init; } = "?";
}
