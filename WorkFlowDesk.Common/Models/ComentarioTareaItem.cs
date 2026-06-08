namespace WorkFlowDesk.Common.Models;

/// <summary>Elemento de comentario para mostrar en la UI de tareas.</summary>
public class ComentarioTareaItem
{
    public string Contenido { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
