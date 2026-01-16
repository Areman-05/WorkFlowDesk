namespace WorkFlowDesk.Domain.Entities;

public enum EstadoProyecto
{
    Planificacion = 1,
    EnProgreso = 2,
    EnPausa = 3,
    Completado = 4,
    Cancelado = 5
}

public class Proyecto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoProyecto Estado { get; set; } = EstadoProyecto.Planificacion;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public int? ClienteId { get; set; }
    public int? ResponsableId { get; set; }

    // Navegación
    public Cliente? Cliente { get; set; }
    public Empleado? Responsable { get; set; }
}
