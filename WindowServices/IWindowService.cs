using System.Windows;

namespace libQB.WindowServices;

public interface IWindowService
{
    void ShowWindow<TWindow, TViewModel>(object parameter = null!, Window owner = null!)
        where TWindow : Window, new()
        where TViewModel : class;

    void ShowWindowWithCallback<TWindow, TViewModel, TResult>
    (
        object parameter = null!,
        Window owner = null!,
        Action<TResult> resultCallback = null!
    )
        where TWindow : Window, new()
        where TViewModel : class;

    void CloseWindow(Window window);
}
