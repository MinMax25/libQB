using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;

namespace libQB.DialogServices;

public partial class GenericDialogViewModel
    : ObservableObject
{
    [ObservableProperty] private string title = "";
    [ObservableProperty] private string message = "";
    [ObservableProperty] private PackIconMaterialKind iconKind;
    [ObservableProperty] private Brush iconColor = Brushes.Gray;
    [ObservableProperty] private string okButtonText = DialogTextProvider.OkButtonText();
    [ObservableProperty] private string cancelButtonText = DialogTextProvider.CancelButtonText();
    [ObservableProperty] private bool showCancel = true;

    public TaskCompletionSource<bool?> CompletionSource { get; } = new();

    [RelayCommand]
    private void Ok() => CompletionSource.TrySetResult(true);

    [RelayCommand]
    private void Cancel() => CompletionSource.TrySetResult(false);
}
