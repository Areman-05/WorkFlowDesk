namespace WorkFlowDesk.Common.Services;

/// <summary>Datos de una sesión almacenada en el perfil del usuario.</summary>
public class SesionPerfilData
{
    public string Id { get; set; } = string.Empty;
    public string Dispositivo { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public string Icono { get; set; } = "\uE770";
    public DateTime UltimaActividad { get; set; } = DateTime.Now;
}

/// <summary>Preferencias de perfil persistidas por usuario.</summary>
public class UserProfileData
{
    public string Telefono { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = "Madrid, España";
    public string Idioma { get; set; } = "Español (España)";
    public string Tema { get; set; } = "Claro";
    public bool NotificacionesEscritorio { get; set; } = true;
    public bool AutenticacionDosFactores { get; set; }
    public string? PinSecundario { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public List<SesionPerfilData> Sesiones { get; set; } = new();
    public List<string> SesionesRevocadas { get; set; } = new();
}
