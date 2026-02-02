using System;
using System.Globalization;
using System.Windows.Data;
using WorkFlowDesk.Common.Helpers;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Convierte un enum a su descripción (atributo Description) para binding.</summary>
public class EnumToDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return string.Empty;

        if (value is Enum enumValue)
        {
            return EnumHelper.GetDescription(enumValue);
        }

        return value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
