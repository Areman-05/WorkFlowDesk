using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Convierte null a Collapsed y no null a Visible para mostrar/ocultar controles.</summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Visibility.Collapsed;

        if (value is string text && string.IsNullOrWhiteSpace(text))
            return Visibility.Collapsed;

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
