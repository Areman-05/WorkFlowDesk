namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Sincronización entre equipos vía carpeta compartida.</summary>
public interface ISyncService
{
    string? CarpetaCompartida { get; set; }
    Task<DateTime?> GetUltimaSyncAsync();
    Task<SyncResult> ExportarCambiosAsync();
    Task<SyncResult> ImportarCambiosAsync();
}

public sealed class SyncResult
{
    public bool Exito { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    public int RegistrosAplicados { get; init; }
}
