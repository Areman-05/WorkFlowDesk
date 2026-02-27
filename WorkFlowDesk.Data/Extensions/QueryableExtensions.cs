using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace WorkFlowDesk.Data.Extensions;

/// <summary>Extensiones para IQueryable (Include múltiple, WhereIf, Paginate).</summary>
public static class QueryableExtensions
{
    /// <summary>Aplica varios Include a la consulta.</summary>
    public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes) where T : class
    {
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    /// <summary>Aplica el predicado solo si la condición es true.</summary>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>Omite y toma los ítems de la página indicada.</summary>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
