namespace WorkFlowDesk.Common.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<TDto> ToDtoList<TEntity, TDto>(
        this IEnumerable<TEntity> entities,
        Func<TEntity, TDto> mapper)
    {
        return entities.Select(mapper);
    }

    public static List<TDto> ToDtoList<TEntity, TDto>(
        this List<TEntity> entities,
        Func<TEntity, TDto> mapper)
    {
        return entities.Select(mapper).ToList();
    }
}
