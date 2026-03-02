using System.Linq.Expressions;

namespace WorkFlowDesk.Data.Repositories;

/// <summary>Contrato del repositorio genérico CRUD.</summary>
public interface IRepository<T> where T : class
{
    /// <summary>Obtiene la entidad por su identificador, o null si no existe.</summary>
    Task<T?> GetByIdAsync(int id);
    /// <summary>Obtiene todas las entidades del conjunto.</summary>
    Task<IEnumerable<T>> GetAllAsync();
    /// <summary>Busca entidades que cumplan el predicado.</summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    /// <summary>Añade la entidad y persiste los cambios.</summary>
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}
