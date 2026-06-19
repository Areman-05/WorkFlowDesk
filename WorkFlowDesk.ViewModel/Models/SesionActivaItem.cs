namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Sesión activa mostrada en el perfil de usuario.</summary>
public class SesionActivaItem
{
    public string Id { get; init; } = string.Empty;
    public string Dispositivo { get; init; } = string.Empty;
    public string Detalle { get; init; } = string.Empty;
    public string Icono { get; init; } = "\uE770";
    public bool EsActual { get; init; }
    public bool EsDemo { get; init; } = true;
}
