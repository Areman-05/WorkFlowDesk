using Microsoft.EntityFrameworkCore;

namespace WorkFlowDesk.Data.Extensions;

/// <summary>Extensiones de consulta EF Core para lecturas sin seguimiento.</summary>
public static class EfQueryExtensions
{
    public static IQueryable<T> AsReadOnly<T>(this IQueryable<T> query) where T : class =>
        query.AsNoTracking();
}
