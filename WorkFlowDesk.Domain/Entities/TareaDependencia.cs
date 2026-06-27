namespace WorkFlowDesk.Domain.Entities;

/// <summary>Dependencia entre tareas (bloqueo hasta completar otra).</summary>
public class TareaDependencia
{
    public int Id { get; set; }
    public int TareaId { get; set; }
    public int DependeDeTareaId { get; set; }

    public Tarea Tarea { get; set; } = null!;
    public Tarea DependeDe { get; set; } = null!;
}
