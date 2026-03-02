namespace WorkFlowDesk.Common.Models;

/// <summary>DTO de empleado para listados y exportación.</summary>
public class EmpleadoDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
