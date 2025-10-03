using System.Globalization;
using System.Windows.Data;

namespace libQB.ValueConverters;

public class IncrementConverter
    : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return int.Parse($"0{value}") + 1;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
