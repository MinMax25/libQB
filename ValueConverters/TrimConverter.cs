using System.Globalization;
using System.Windows.Data;

namespace libQB.ValueConverters;

public class TrimConverter
    : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString()?.Trim();
    }
}
