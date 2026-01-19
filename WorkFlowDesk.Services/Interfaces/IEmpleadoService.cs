using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

public interface IEmpleadoService
{
    Task<Empleado?> GetByIdAsync(int id);
    Task<IEnumerable<Empleado>> GetAllAsync();
    Task<IEnumerable<Empleado>> GetActivosAsync();
    Task<Empleado> CreateAsync(Empleado empleado);
    Task UpdateAsync(Empleado empleado);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string email);
}
