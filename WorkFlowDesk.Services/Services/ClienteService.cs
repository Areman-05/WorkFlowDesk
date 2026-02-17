using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de gestión de clientes.</summary>
public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;

    public ClienteService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene un cliente por ID con sus proyectos cargados.</summary>
    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.Proyectos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>Obtiene todos los clientes ordenados por nombre.</summary>
    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cliente>> GetActivosAsync()
    {
        return await _context.Clientes
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        cliente.FechaCreacion = DateTime.Now;
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            cliente.Activo = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Clientes.AnyAsync(c => c.Email == email);
    }
}
