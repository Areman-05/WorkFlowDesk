using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

public interface IClienteService
{
    Task<Cliente?> GetByIdAsync(int id);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<IEnumerable<Cliente>> GetActivosAsync();
    Task<Cliente> CreateAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string email);
}
