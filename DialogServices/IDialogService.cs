namespace libQB.DialogServices;

public interface IDialogService
{
    #region CommonDialog

    Task<string> ShowOpenFileDialog(string title, string filter = "", string path = "");

    Task<string> ShowSaveFileDialog(string title, string filter = "", string path = "");

    Task<string> ShowSelectFolderDialog(string title, string path = "");

    #endregion

    #region Generic Dialog

    Task<bool?> ShowGenericDialogAsync(GenericDialogOptions options);

    Task ShowInformationAsync(string message, string title = null!);

    Task ShowAlertAsync(string message, string title = null!);

    Task ShowErrorAsync(string message, string title = null!);

    Task<bool?> ShowWarningAsync(string message, string title = null!);

    Task<bool?> ShowConfirmAsync(string message, string title = null!);

    #endregion
}