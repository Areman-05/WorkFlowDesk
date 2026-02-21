namespace WorkFlowDesk.Domain.Entities;

/// <summary>Entidad de cliente (empresa o persona que encarga proyectos).</summary>
public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
