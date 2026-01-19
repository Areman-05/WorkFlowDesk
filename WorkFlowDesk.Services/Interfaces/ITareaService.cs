using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

public interface ITareaService
{
    Task<Tarea?> GetByIdAsync(int id);
    Task<IEnumerable<Tarea>> GetAllAsync();
    Task<IEnumerable<Tarea>> GetByEstadoAsync(EstadoTarea estado);
    Task<IEnumerable<Tarea>> GetByProyectoAsync(int proyectoId);
    Task<IEnumerable<Tarea>> GetByAsignadoAsync(int empleadoId);
    Task<Tarea> CreateAsync(Tarea tarea);
    Task UpdateAsync(Tarea tarea);
    Task DeleteAsync(int id);
    Task AgregarComentarioAsync(int tareaId, ComentarioTarea comentario);
}
