using System.Globalization;
using System.Windows.Data;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.UI.Converters;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Convierte un índice de avatar (0–11) en BitmapImage.</summary>
public class AvatarIndexToImageConverter : IValueConverter
{
    private static readonly UrlToBitmapConverter UrlConverter = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int index)
            return null;

        return UrlConverter.Convert(AvatarCatalog.GetUrl(index), targetType, parameter, culture);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
