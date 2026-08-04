using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace libQB.DialogServices;

public class GenericDialogOptions
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public PackIconMaterialKind IconKind { get; set; }
    public Brush IconColor { get; set; } = Brushes.Gray;
    public string OkButtonText { get; set; } = DialogTextProvider.OkButtonText();
    public string CancelButtonText { get; set; } = DialogTextProvider.CancelButtonText();
    public bool ShowCancelButton { get; set; } = false;

    public static GenericDialogOptions Create(DialogType type, string message, string? title = null)
    {
        return type switch
        {
            DialogType.Information => new GenericDialogOptions
            {
                Title = title ?? DialogTextProvider.InformationTitle(),
                Message = message,
                IconKind = PackIconMaterialKind.InformationOutline,
                IconColor = Brushes.SkyBlue
            },
            DialogType.Alert => new GenericDialogOptions
            {
                Title = title ?? DialogTextProvider.WarningTitle(),
                Message = message,
                IconKind = PackIconMaterialKind.BellAlertOutline,
                IconColor = Brushes.Orange,
            },
            DialogType.Warning => new GenericDialogOptions
            {
                Title = title ?? DialogTextProvider.WarningTitle(),
                Message = message,
                IconKind = PackIconMaterialKind.ShieldAlertOutline,
                IconColor = Brushes.Orange,
                ShowCancelButton = true
            },
            DialogType.Error => new GenericDialogOptions
            {
                Title = title ?? DialogTextProvider.ErrorTitle(),
                Message = message,
                IconKind = PackIconMaterialKind.CloseCircleOutline,
                IconColor = Brushes.Red
            },
            DialogType.Confirm => new GenericDialogOptions
            {
                Title = title ?? DialogTextProvider.ConfirmTitle(),
                Message = message,
                IconKind = PackIconMaterialKind.HelpCircleOutline,
                IconColor = Brushes.DodgerBlue,
                OkButtonText = DialogTextProvider.YesButtonText(),
                CancelButtonText = DialogTextProvider.NoButtonText(),
                ShowCancelButton = true
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
