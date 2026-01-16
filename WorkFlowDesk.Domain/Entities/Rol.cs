namespace WorkFlowDesk.Domain.Entities;

public enum TipoRol
{
    Admin = 1,
    Supervisor = 2,
    Empleado = 3
}

public class Rol
{
    public int Id { get; set; }
    public TipoRol TipoRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
