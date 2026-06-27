namespace WorkFlowDesk.Domain.Entities;

/// <summary>Archivo adjunto a una tarea (metadatos; el binario vive en disco).</summary>
public class TareaAdjunto
{
    public int Id { get; set; }
    public int TareaId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaRelativa { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.Now;
    public int? SubidoPorEmpleadoId { get; set; }

    public Tarea Tarea { get; set; } = null!;
    public Empleado? SubidoPor { get; set; }
}
