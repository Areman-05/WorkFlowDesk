namespace WorkFlowDesk.Common.Models;

public class TareaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string AsignadoNombre { get; set; } = string.Empty;
    public string ProyectoNombre { get; set; } = string.Empty;
    public DateTime? FechaVencimiento { get; set; }
}
