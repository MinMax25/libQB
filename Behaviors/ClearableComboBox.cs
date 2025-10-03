namespace libQB.Behaviors;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public static class ClearableComboBox
{
    public static readonly DependencyProperty EnableClearButtonProperty =
        DependencyProperty.RegisterAttached(
            "EnableClearButton",
            typeof(bool),
            typeof(ClearableComboBox),
            new PropertyMetadata(false, OnEnableClearButtonChanged));

    public static void SetEnableClearButton(DependencyObject element, bool value) =>
        element.SetValue(EnableClearButtonProperty, value);

    public static bool GetEnableClearButton(DependencyObject element) =>
        (bool)element.GetValue(EnableClearButtonProperty);

    public static readonly DependencyProperty ClearValueProperty =
        DependencyProperty.RegisterAttached(
            "ClearValue",
            typeof(object),
            typeof(ClearableComboBox),
            new PropertyMetadata(null));

    public static void SetClearValue(DependencyObject element, object value) =>
        element.SetValue(ClearValueProperty, value);

    public static object GetClearValue(DependencyObject element) =>
        element.GetValue(ClearValueProperty);

    private static void OnEnableClearButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ComboBox comboBox)
        {
            if ((bool)e.NewValue)
                comboBox.Loaded += ComboBox_Loaded;
            else
                comboBox.Loaded -= ComboBox_Loaded;
        }
    }

    private static void ComboBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox comboBox) return;

        comboBox.ApplyTemplate();

        var rootGrid = FindVisualChild<Grid>(comboBox);
        if (rootGrid == null) return;

        if (rootGrid.Children.OfType<Button>().Any(b => b.Name == "PART_ClearButton")) return;

        var clearButton = new Button
        {
            Name = "PART_ClearButton",
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            Content = "\uE711",
            Width = 20,
            Height = 20,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Foreground = Brushes.Gray,
            ToolTip = Properties.Dialog.Title_Command_Clear,
            Cursor = Cursors.Hand,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 24, 0),
            Visibility = comboBox.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed,
            Focusable = false
        };

        clearButton.Click += (s, args) =>
        {
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
            else
            {
                comboBox.SelectedIndex = -1;
            }

            comboBox.Focus();
        };

        comboBox.SelectionChanged += (s, args) =>
        {
            clearButton.Visibility = comboBox.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
        };

        Panel.SetZIndex(clearButton, 99);
        rootGrid.Children.Add(clearButton);
    }

    private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T tChild)
                return tChild;

            var result = FindVisualChild<T>(child);
            if (result != null)
                return result;
        }

        return null!;
    }
}
