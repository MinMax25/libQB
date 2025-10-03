using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace libQB.Behaviors;

public class ContextMenuBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty IsContextMenuEnabledProperty =
        DependencyProperty.Register(
            nameof(IsContextMenuEnabled),
            typeof(bool),
            typeof(ContextMenuBehavior),
            new PropertyMetadata(true));

    public bool IsContextMenuEnabled
    {
        get => (bool)GetValue(IsContextMenuEnabledProperty);
        set => SetValue(IsContextMenuEnabledProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.ContextMenuOpening += OnContextMenuOpening;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.ContextMenuOpening -= OnContextMenuOpening;
        base.OnDetaching();
    }

    private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (!IsContextMenuEnabled)
        {
            e.Handled = true;
        }
    }
}
