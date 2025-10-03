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
    [ObservableProperty] private string okButtonText = Properties.Dialog.Button_OK;
    [ObservableProperty] private string cancelButtonText = Properties.Dialog.Button_Cancel;
    [ObservableProperty] private bool showCancel = true;

    public TaskCompletionSource<bool?> CompletionSource { get; } = new();

    [RelayCommand]
    private void Ok() => CompletionSource.TrySetResult(true);

    [RelayCommand]
    private void Cancel() => CompletionSource.TrySetResult(false);
}
