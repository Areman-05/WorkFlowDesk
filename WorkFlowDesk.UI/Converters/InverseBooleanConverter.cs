using System.Globalization;
using System.Windows.Data;

namespace WorkFlowDesk.UI;

/// <summary>Invierte un valor booleano para binding (ej. IsEnabled vs IsBusy).</summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return false;
    }
}
