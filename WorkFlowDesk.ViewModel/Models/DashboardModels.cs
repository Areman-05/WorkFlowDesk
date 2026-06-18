namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Proyecto en progreso mostrado en el dashboard.</summary>
public class DashboardProyectoItem
{
    public string Nombre { get; init; } = string.Empty;
    public int ProgresoPorcentaje { get; init; }
    public string BarColor { get; init; } = "#9E00B5";
}

/// <summary>Fila de proyecto prioritario en la tabla del dashboard.</summary>
public class DashboardProyectoPrioritarioItem
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Subtitulo { get; init; } = string.Empty;
    public string Cliente { get; init; } = string.Empty;
    public string EstadoTexto { get; init; } = string.Empty;
    public string EstadoFondo { get; init; } = "#E3DFFF";
    public string EstadoTextoColor { get; init; } = "#181445";
    public string Inicial { get; init; } = "?";
    public string AvatarFondo { get; init; } = "#FBABFF";
}

/// <summary>Evento de actividad reciente en el dashboard.</summary>
public class DashboardActividadItem
{
    public string Icono { get; init; } = "\uE710";
    public string IconoFondo { get; init; } = "#1A9E00B5";
    public string IconoColor { get; init; } = "#9E00B5";
    public string Texto { get; init; } = string.Empty;
    public string TiempoRelativo { get; init; } = string.Empty;
    public DateTime Fecha { get; init; }
}
