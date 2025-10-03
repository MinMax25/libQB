using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace libQB.DialogServices;

public class GenericDialogOptions
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public PackIconMaterialKind IconKind { get; set; }
    public Brush IconColor { get; set; } = Brushes.Gray;
    public string OkButtonText { get; set; } = Properties.Dialog.Button_OK;
    public string CancelButtonText { get; set; } = Properties.Dialog.Button_Cancel;
    public bool ShowCancelButton { get; set; } = false;

    public static GenericDialogOptions Create(DialogType type, string message, string? title = null)
    {
        return type switch
        {
            DialogType.Information => new GenericDialogOptions
            {
                Title = title ?? Properties.Dialog.Title_Dialog_Infomation,
                Message = message,
                IconKind = PackIconMaterialKind.InformationOutline,
                IconColor = Brushes.SkyBlue
            },
            DialogType.Alert => new GenericDialogOptions
            {
                Title = title ?? Properties.Dialog.Title_Dialog_Warning,
                Message = message,
                IconKind = PackIconMaterialKind.BellAlertOutline,
                IconColor = Brushes.Orange,
            },
            DialogType.Warning => new GenericDialogOptions
            {
                Title = title ?? Properties.Dialog.Title_Dialog_Warning,
                Message = message,
                IconKind = PackIconMaterialKind.ShieldAlertOutline,
                IconColor = Brushes.Orange,
                ShowCancelButton = true
            },
            DialogType.Error => new GenericDialogOptions
            {
                Title = title ?? Properties.Dialog.Title_Dialog_Error,
                Message = message,
                IconKind = PackIconMaterialKind.CloseCircleOutline,
                IconColor = Brushes.Red
            },
            DialogType.Confirm => new GenericDialogOptions
            {
                Title = title ?? Properties.Dialog.Title_Dialog_Confirm,
                Message = message,
                IconKind = PackIconMaterialKind.HelpCircleOutline,
                IconColor = Brushes.DodgerBlue,
                OkButtonText = Properties.Dialog.Button_Yes,
                CancelButtonText = Properties.Dialog.Button_No,
                ShowCancelButton = true
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
