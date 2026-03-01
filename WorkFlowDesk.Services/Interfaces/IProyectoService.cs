using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Servicio de gestión de proyectos (CRUD y consultas por estado).</summary>
public interface IProyectoService
{
    Task<Proyecto?> GetByIdAsync(int id);
    Task<IEnumerable<Proyecto>> GetAllAsync();
    Task<IEnumerable<Proyecto>> GetByEstadoAsync(EstadoProyecto estado);
    Task<Proyecto> CreateAsync(Proyecto proyecto);
    Task UpdateAsync(Proyecto proyecto);
    Task DeleteAsync(int id);
}
