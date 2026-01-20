namespace WorkFlowDesk.Common.Models;

public class EmpleadoDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
