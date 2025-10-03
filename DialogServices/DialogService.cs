namespace libQB.DialogServices;

using System.IO;
using System.Reflection;
using System.Windows;
using libQB.Attributes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

[DISingleton<IDialogService>]
public class DialogService
    : IDialogService
{
    private MetroWindow? mainWindow => Application.Current.MainWindow as MetroWindow;

    #region ctor

    public DialogService()
    {
    }

    #endregion

    #region CommonDialog

    public Task<string> ShowOpenFileDialog(string? title, string? filter = "", string? path = "")
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = false,
            Title = title,
            InitialDirectory = FixDirectoryName(path ?? string.Empty),
            FileName = FixFileName(path ?? string.Empty),
            Filter = filter
        };

        var result = dialog.ShowDialog() == true;
        return Task.FromResult(result ? dialog.FileName : null!);
    }

    public Task<string> ShowSaveFileDialog(string? title, string? filter = "", string? path = "")
    {
        var dialog = new SaveFileDialog
        {
            Title = title,
            InitialDirectory = FixDirectoryName(path ?? string.Empty),
            FileName = FixFileName(path ?? string.Empty),
            Filter = filter
        };

        var result = dialog.ShowDialog() == true;
        return Task.FromResult(result ? dialog.FileName : null!);
    }

    public Task<string> ShowSelectFolderDialog(string? title, string path = "")
    {
        var dialog = new OpenFolderDialog
        {
            Title = title,
            InitialDirectory = FixDirectoryName(path)
        };

        var result = dialog.ShowDialog() == true;
        return Task.FromResult(result ? dialog.FolderName : null!);
    }

    private string FixDirectoryName(string path)
    {
        return (File.Exists(path) ? Path.GetDirectoryName(path) : Directory.Exists(path) ? path : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) ?? string.Empty;
    }

    private string FixFileName(string path)
    {
        return File.Exists(path) ? Path.GetFileName(path) : string.Empty;
    }

    #endregion

    #region Generic Dialog

    public async Task<bool?> ShowGenericDialogAsync(GenericDialogOptions options)
    {
        if (mainWindow == null) return null;

        var vm = new GenericDialogViewModel
        {
            Title = options.Title,
            Message = options.Message,
            IconKind = options.IconKind,
            IconColor = options.IconColor,
            OkButtonText = options.OkButtonText,
            CancelButtonText = options.CancelButtonText,
            ShowCancel = options.ShowCancelButton
        };

        var dialog = new GenericDialog { DataContext = vm };
        await mainWindow.ShowMetroDialogAsync(dialog);
        var result = await vm.CompletionSource.Task;
        await mainWindow.HideMetroDialogAsync(dialog);
        return result;
    }

    public Task ShowInformationAsync(string message, string title = null!)
        => ShowGenericDialogAsync(GenericDialogOptions.Create(DialogType.Information, message, title));

    public Task ShowAlertAsync(string message, string title = null!)
        => ShowGenericDialogAsync(GenericDialogOptions.Create(DialogType.Alert, message, title));

    public Task ShowErrorAsync(string message, string title = null!)
        => ShowGenericDialogAsync(GenericDialogOptions.Create(DialogType.Error, message, title));

    public Task<bool?> ShowWarningAsync(string message, string title = null!)
        => ShowGenericDialogAsync(GenericDialogOptions.Create(DialogType.Warning, message, title));

    public Task<bool?> ShowConfirmAsync(string message, string title = null!)
        => ShowGenericDialogAsync(GenericDialogOptions.Create(DialogType.Confirm, message, title));

    #endregion
}
