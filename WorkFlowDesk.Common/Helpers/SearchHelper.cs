using System.Linq.Expressions;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Filtrado de colecciones por texto o propiedad.</summary>
public static class SearchHelper
{
    public static IEnumerable<T> FilterByText<T>(IEnumerable<T> items, string searchText, params Func<T, string>[] propertySelectors)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return items;

        searchText = searchText.ToLowerInvariant();

        return items.Where(item =>
            propertySelectors.Any(selector =>
            {
                var value = selector(item);
                return value?.ToLowerInvariant().Contains(searchText) ?? false;
            })
        );
    }

    public static IEnumerable<T> FilterByProperty<T, TProperty>(
        IEnumerable<T> items,
        Expression<Func<T, TProperty>> propertySelector,
        TProperty? value) where TProperty : IEquatable<TProperty>
    {
        if (value == null)
            return items;

        var compiledSelector = propertySelector.Compile();
        return items.Where(item => compiledSelector(item).Equals(value));
    }
}
