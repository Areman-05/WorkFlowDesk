using System.Linq.Expressions;

namespace WorkFlowDesk.Data.Repositories;

/// <summary>Contrato del repositorio genérico CRUD.</summary>
public interface IRepository<T> where T : class
{
    /// <summary>Obtiene la entidad por su identificador, o null si no existe.</summary>
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}
