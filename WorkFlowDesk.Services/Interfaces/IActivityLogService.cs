namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Historial de actividad auditable.</summary>
public interface IActivityLogService
{
    Task RegistrarAsync(string entidad, int? entidadId, string accion, string detalle);
    Task<IReadOnlyList<ActivityLogEntry>> GetRecientesAsync(int limite = 50);
    Task<IReadOnlyList<ActivityLogEntry>> GetPorEntidadAsync(string entidad, int entidadId, int limite = 30);
}

public sealed class ActivityLogEntry
{
    public int Id { get; init; }
    public DateTime FechaUtc { get; init; }
    public string Usuario { get; init; } = string.Empty;
    public string Entidad { get; init; } = string.Empty;
    public int? EntidadId { get; init; }
    public string Accion { get; init; } = string.Empty;
    public string Detalle { get; init; } = string.Empty;
}
