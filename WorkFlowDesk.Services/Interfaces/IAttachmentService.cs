namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Adjuntos de tareas en disco local.</summary>
public interface IAttachmentService
{
    Task<IReadOnlyList<TareaAdjuntoInfo>> GetByTareaAsync(int tareaId);
    Task<TareaAdjuntoInfo> AgregarAsync(int tareaId, string rutaOrigen, int? empleadoId);
    Task<bool> EliminarAsync(int adjuntoId);
    string ObtenerRutaCompleta(string rutaRelativa);
}

public sealed class TareaAdjuntoInfo
{
    public int Id { get; init; }
    public int TareaId { get; init; }
    public string NombreArchivo { get; init; } = string.Empty;
    public string RutaRelativa { get; init; } = string.Empty;
    public long TamanoBytes { get; init; }
    public DateTime FechaSubida { get; init; }
}
