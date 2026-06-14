namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Proyecto en progreso mostrado en el dashboard.</summary>
public class DashboardProyectoItem
{
    public string Nombre { get; init; } = string.Empty;
    public int ProgresoPorcentaje { get; init; }
}
