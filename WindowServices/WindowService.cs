namespace libQB.WindowServices;

using System.Windows;
using libQB.Attributes;

[DISingleton<IWindowService>]
public class WindowService
    : IWindowService
{
    public Window Owner { get; set; } = null!;

    private readonly IServiceProvider _serviceProvider;

    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public void ShowWindow<TWindow, TViewModel>(object parameter = null!, Window owner = null!)
        where TWindow : Window, new()
        where TViewModel : class
    {
        try
        {
            var window = new TWindow();

            var viewModel =
                _serviceProvider.GetService(typeof(TViewModel))
                ?? throw new InvalidOperationException($"Failed to resolve {typeof(TViewModel).Name}");

            window.DataContext = viewModel;

            if (owner != null)
            {
                window.Owner = owner;
            }

            window.ShowDialog();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void ShowWindowWithCallback<TWindow, TViewModel, TResult>
    (
        object parameter = null!,
        Window owner = null!,
        Action<TResult> resultCallback = null!
    )
        where TWindow : Window, new()
        where TViewModel : class
    {
        try
        {
            var window = new TWindow();
            var viewModel = _serviceProvider.GetService(typeof(TViewModel));

            if (viewModel == null)
            {
                throw new InvalidOperationException($"Failed to resolve {typeof(TViewModel).Name}");
            }

            if (viewModel is IParameterReceiver parameterReceiver)
            {
                parameterReceiver.ReceiveParameter(parameter);
            }

            window.DataContext = viewModel;

            if (owner != null)
            {
                window.Owner = owner;
            }

            window.Closed += (sender, args) =>
            {
                if (viewModel is IResultProvider<TResult> resultProvider)
                {
                    resultCallback?.Invoke(resultProvider.GetResult());
                }
                else
                {
                    resultCallback?.Invoke(default!);
                }
            };

            window.ShowDialog();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void CloseWindow(Window window)
    {
        window.Close();
    }
}
