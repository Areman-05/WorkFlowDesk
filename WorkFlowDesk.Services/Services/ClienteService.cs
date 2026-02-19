using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de gestión de clientes.</summary>
public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;

    /// <summary>Inicializa el servicio con el contexto de base de datos.</summary>
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

    /// <summary>Obtiene solo los clientes activos ordenados por nombre.</summary>
    public async Task<IEnumerable<Cliente>> GetActivosAsync()
    {
        return await _context.Clientes
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    /// <summary>Crea un nuevo cliente en la base de datos.</summary>
    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        cliente.FechaCreacion = DateTime.Now;
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    /// <summary>Actualiza los datos del cliente en la base de datos.</summary>
    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    /// <summary>Desactiva el cliente (borrado lógico) por ID.</summary>
    public async Task DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            cliente.Activo = false;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Comprueba si existe un cliente con el email indicado.</summary>
    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Clientes.AnyAsync(c => c.Email == email);
    }
}
