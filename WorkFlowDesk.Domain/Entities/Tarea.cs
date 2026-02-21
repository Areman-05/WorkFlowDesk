namespace WorkFlowDesk.Domain.Entities;

public enum PrioridadTarea
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum EstadoTarea
{
    Pendiente = 1,
    EnProgreso = 2,
    EnRevision = 3,
    Completada = 4,
    Cancelada = 5
}

/// <summary>Entidad de tarea (trabajo asignable con prioridad y estado).</summary>
public class Tarea
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Media;
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public int? AsignadoId { get; set; }
    public int? CreadorId { get; set; }
    public int? ProyectoId { get; set; }

    // Navegación
    public Empleado? Asignado { get; set; }
    public Empleado? Creador { get; set; }
    public Proyecto? Proyecto { get; set; }
    public ICollection<ComentarioTarea> Comentarios { get; set; } = new List<ComentarioTarea>();
}

public class ComentarioTarea
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public int TareaId { get; set; }
    public int? EmpleadoId { get; set; }

    // Navegación
    public Tarea Tarea { get; set; } = null!;
    public Empleado? Empleado { get; set; }
}
