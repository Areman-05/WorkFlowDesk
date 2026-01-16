namespace WorkFlowDesk.Domain.Entities;

public enum EstadoEmpleado
{
    Activo = 1,
    Inactivo = 2,
    Vacaciones = 3,
    Baja = 4
}

public class Empleado
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public EstadoEmpleado Estado { get; set; } = EstadoEmpleado.Activo;
    public DateTime FechaContratacion { get; set; }
    public DateTime? FechaBaja { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public int? UsuarioId { get; set; }

    // Navegación
    public Usuario? Usuario { get; set; }
}
