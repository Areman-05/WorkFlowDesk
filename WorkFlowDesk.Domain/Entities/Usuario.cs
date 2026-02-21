namespace WorkFlowDesk.Domain.Entities;

/// <summary>Entidad de usuario del sistema (login, rol, perfil).</summary>
public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public int RolId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? UltimoAcceso { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public Rol Rol { get; set; } = null!;
}
