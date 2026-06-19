using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Visible cuando el entero es 0.</summary>
public class IntZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count)
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
