namespace WorkFlowDesk.Domain.Entities;

/// <summary>Subtarea dentro de una tarea principal.</summary>
public class SubTarea
{
    public int Id { get; set; }
    public int TareaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public bool Completada { get; set; }
    public int Orden { get; set; }

    public Tarea Tarea { get; set; } = null!;
}
