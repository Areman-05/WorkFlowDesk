using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Exceptions;
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
        // Validaciones de negocio
        if (await ExistsAsync(empleado.Email))
        {
            throw new ValidationException($"Ya existe un empleado con el email {empleado.Email}");
        }

        if (string.IsNullOrWhiteSpace(empleado.Nombre))
        {
            throw new ValidationException("El nombre del empleado es requerido");
        }

        if (string.IsNullOrWhiteSpace(empleado.Apellidos))
        {
            throw new ValidationException("Los apellidos del empleado son requeridos");
        }

        empleado.FechaCreacion = DateTime.Now;
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();
        return empleado;
    }

    public async Task UpdateAsync(Empleado empleado)
    {
        var existing = await GetByIdAsync(empleado.Id);
        if (existing == null)
        {
            throw new EntityNotFoundException("Empleado", empleado.Id);
        }

        // Validar que el email no esté duplicado
        if (await _context.Empleados.AnyAsync(e => e.Email == empleado.Email && e.Id != empleado.Id))
        {
            throw new ValidationException($"Ya existe otro empleado con el email {empleado.Email}");
        }

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
