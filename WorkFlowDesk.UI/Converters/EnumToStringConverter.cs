using System;
using System.Globalization;
using System.Windows.Data;
using WorkFlowDesk.Common.Helpers;

namespace WorkFlowDesk.UI.Converters;

public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return string.Empty;

        if (value is Enum enumValue)
        {
            return EnumHelper.ToDisplayString(enumValue);
        }

        return value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
        {
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, stringValue.Replace(" ", "_"), true);
            }
        }

        return value ?? Activator.CreateInstance(targetType)!;
    }
}
