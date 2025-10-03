namespace libQB.Behaviors;

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

public class ClosePopupOnOutsideClickBehavior
    : Behavior<Popup>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Opened += OnOpened;
        AssociatedObject.Closed += OnClosed;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (Application.Current.MainWindow != null)
            Application.Current.MainWindow.PreviewMouseDown += MainWindow_PreviewMouseDown;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (Application.Current.MainWindow != null)
            Application.Current.MainWindow.PreviewMouseDown -= MainWindow_PreviewMouseDown;
    }

    private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (AssociatedObject.Child is FrameworkElement popupRoot)
        {
            var point = e.GetPosition(popupRoot);
            var rect = new Rect(0, 0, popupRoot.ActualWidth, popupRoot.ActualHeight);
            if (!rect.Contains(point))
            {
                AssociatedObject.IsOpen = false;
            }
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Opened -= OnOpened;
        AssociatedObject.Closed -= OnClosed;
    }
}
