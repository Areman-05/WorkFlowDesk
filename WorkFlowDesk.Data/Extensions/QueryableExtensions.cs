using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace WorkFlowDesk.Data.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes) where T : class
    {
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
