using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Convierte longitud de arco en StrokeDashArray para gráficos donut.</summary>
public class ArcToDashArrayConverter : IValueConverter
{
    private const double Circunferencia = 251.2;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var arco = value switch
        {
            double d => d,
            int i => (double)i,
            _ => 0d
        };

        return new DoubleCollection { arco, Circunferencia };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
