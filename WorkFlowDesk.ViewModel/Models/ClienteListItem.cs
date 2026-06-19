using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Fila de listado de clientes para la vista Stitch.</summary>
public class ClienteListItem
{
    public required Cliente Cliente { get; init; }
    public string NombreDisplay { get; init; } = string.Empty;
    public string CodigoId { get; init; } = string.Empty;
    public string Sector { get; init; } = "Servicios";
    public string SectorIcon { get; init; } = "\uE821";
    public string SectorIconFondo { get; init; } = "#266B7280";
    public string SectorIconColor { get; init; } = "#524251";
    public string ContactoNombre { get; init; } = string.Empty;
    public string ContactoEmail { get; init; } = string.Empty;
    public string EstadoTexto { get; init; } = "Activo";
    public string EstadoFondo { get; init; } = "#DCFCE7";
    public string EstadoTextoColor { get; init; } = "#15803D";
    public string EstadoDotColor { get; init; } = "#22C55E";
    public int ProyectosCount { get; init; }
    public string ProyectosEtiqueta { get; init; } = "proyectos";
}
