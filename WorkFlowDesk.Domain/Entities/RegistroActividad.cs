namespace WorkFlowDesk.Domain.Entities;

/// <summary>Entrada del historial de actividad del sistema.</summary>
public class RegistroActividad
{
    public int Id { get; set; }
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
    public int? UsuarioId { get; set; }
    public int? EmpleadoId { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;

    public Usuario? Usuario { get; set; }
    public Empleado? Empleado { get; set; }
}
