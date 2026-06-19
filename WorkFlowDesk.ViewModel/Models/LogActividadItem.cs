namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Línea de actividad del sistema en configuración.</summary>
public class LogActividadItem
{
    public string Hora { get; init; } = string.Empty;
    public string Mensaje { get; init; } = string.Empty;
    public bool EsAdvertencia { get; init; }
}
