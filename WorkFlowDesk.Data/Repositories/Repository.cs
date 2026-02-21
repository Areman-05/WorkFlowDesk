using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data.Repositories;

namespace WorkFlowDesk.Data.Repositories;

/// <summary>Repositorio genérico CRUD con Entity Framework.</summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    /// <summary>Inicializa el repositorio con el contexto de base de datos.</summary>
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>Obtiene la entidad por su ID.</summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>Obtiene todas las entidades del conjunto.</summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>Busca entidades que cumplan el predicado.</summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>Añade la entidad y persiste los cambios.</summary>
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Actualiza la entidad y persiste los cambios.</summary>
    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>Elimina la entidad y persiste los cambios.</summary>
    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>Comprueba si existe al menos una entidad que cumpla el predicado.</summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}
