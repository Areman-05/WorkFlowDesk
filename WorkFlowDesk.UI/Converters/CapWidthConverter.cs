using System.Globalization;
using System.Windows.Data;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Limita un ancho al mínimo entre el valor recibido y el parámetro (p. ej. 420).</summary>
public class CapWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var max = 420.0;
        if (parameter is string s && double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            max = parsed;
        else if (parameter is double d)
            max = d;

        if (value is double width && !double.IsNaN(width) && width > 0)
            return Math.Min(width, max);

        return max;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
