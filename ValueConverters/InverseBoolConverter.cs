using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace libQB.ValueConverters;

public class InverseBoolConverter
    : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is bool b)) { return DependencyProperty.UnsetValue; }
        return !b;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is bool b)) { return DependencyProperty.UnsetValue; }
        return !b;
    }
}
