using System.Globalization;
using System.Windows.Data;

namespace libQB.ValueConverters;

public class BaseThemeConverter
    : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && parameter != null)
        {
            return value.ToString()?.Equals(parameter.ToString()) ?? false;
        }
        else
        {
            return false;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && parameter != null && (bool)value)
        {
            return Enum.Parse(targetType, parameter?.ToString() ?? string.Empty);
        }
        else
        {
            return Binding.DoNothing;
        }
    }
}