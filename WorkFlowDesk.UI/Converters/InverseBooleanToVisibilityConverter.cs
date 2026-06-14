using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Invierte bool a Visibility (true → Collapsed).</summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
