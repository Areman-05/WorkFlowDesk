using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly ApplicationDbContext _context;

    public EmpleadoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Empleado?> GetByIdAsync(int id)
    {
        return await _context.Empleados
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _context.Empleados
            .Include(e => e.Usuario)
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Empleado>> GetActivosAsync()
    {
        return await _context.Empleados
            .Include(e => e.Usuario)
            .Where(e => e.Estado == EstadoEmpleado.Activo)
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombre)
            .ToListAsync();
    }

    public async Task<Empleado> CreateAsync(Empleado empleado)
    {
        empleado.FechaCreacion = DateTime.Now;
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();
        return empleado;
    }

    public async Task UpdateAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var empleado = await _context.Empleados.FindAsync(id);
        if (empleado != null)
        {
            empleado.Estado = EstadoEmpleado.Baja;
            empleado.FechaBaja = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Empleados.AnyAsync(e => e.Email == email);
    }
}
