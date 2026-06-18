namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Barra de actividad diaria para el panel semanal.</summary>
public class ActividadDiaItem
{
    public string Etiqueta { get; init; } = string.Empty;
    public int Valor { get; init; }
    public double AlturaPorcentaje { get; init; }
    public double AlturaPixeles => AlturaPorcentaje * 1.2;
    public bool EsDiaActual { get; init; }
}
