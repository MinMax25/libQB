namespace libQB.Behaviors;

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

public static class FocusValidationBlockBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(FocusValidationBlockBehavior),
            new UIPropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject obj)
        => (bool)obj.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject obj, bool value)
        => obj.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element)
        {
            if ((bool)e.NewValue)
            {
                element.PreviewLostKeyboardFocus += OnPreviewLostKeyboardFocus;
            }
            else
            {
                element.PreviewLostKeyboardFocus -= OnPreviewLostKeyboardFocus;
            }
        }
    }

    private static void OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is FrameworkElement element
            && element.DataContext is INotifyDataErrorInfo vm)
        {
            var binding = BindingOperations.GetBindingExpression(element, TextBox.TextProperty);
            if (binding != null)
            {
                string propertyName = binding.ResolvedSourcePropertyName;
                bool hasError = vm.GetErrors(propertyName)?.Cast<object>().Any() ?? false;

                if (hasError)
                {
                    e.Handled = true; // フォーカス移動禁止！
                }
            }
        }
    }
}
