namespace libQB.DialogServices;

// 共通ダイアログ/共通コントロールが使う既定文言の差し込み口。
// libQBはResxを持たないため、既定値は英語のハードコードとし、
// アプリ側が起動時にLocalizationManager等を経由した関数を登録することで言語に追従させる。
public static class DialogTextProvider
{
    public static Func<string> OkButtonText { get; set; } = () => "OK";
    public static Func<string> CancelButtonText { get; set; } = () => "Cancel";
    public static Func<string> YesButtonText { get; set; } = () => "Yes";
    public static Func<string> NoButtonText { get; set; } = () => "No";
    public static Func<string> InformationTitle { get; set; } = () => "Information";
    public static Func<string> WarningTitle { get; set; } = () => "Warning";
    public static Func<string> ErrorTitle { get; set; } = () => "Error";
    public static Func<string> ConfirmTitle { get; set; } = () => "Confirm";
    public static Func<string> ClearButtonTooltip { get; set; } = () => "Clear";
}
