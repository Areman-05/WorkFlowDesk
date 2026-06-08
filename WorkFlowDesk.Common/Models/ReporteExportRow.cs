namespace WorkFlowDesk.Common.Models;

/// <summary>Fila plana para exportar estadísticas de reportes a CSV.</summary>
public class ReporteExportRow
{
    public string Seccion { get; set; } = string.Empty;
    public string Metrica { get; set; } = string.Empty;
    public int Valor { get; set; }
}
