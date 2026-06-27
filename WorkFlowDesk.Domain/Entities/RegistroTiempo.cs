namespace WorkFlowDesk.Domain.Entities;

/// <summary>Registro de tiempo dedicado a una tarea.</summary>
public class RegistroTiempo
{
    public int Id { get; set; }
    public int TareaId { get; set; }
    public int? EmpleadoId { get; set; }
    public int Minutos { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Nota { get; set; } = string.Empty;

    public Tarea Tarea { get; set; } = null!;
    public Empleado? Empleado { get; set; }
}
