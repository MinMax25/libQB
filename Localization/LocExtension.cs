using System.Windows.Data;
using System.Windows.Markup;

namespace libQB.Localization;

// XAMLで {loc:Loc Key=libQB.Menu.File, Fallback={x:Static reslibQB:Menu.File}} のように使用する。
// LocalizationManagerのインデクサへBindingすることで、言語切替(SetCulture)のたびに
// PropertyChanged通知経由で表示中のテキストが自動的に再評価される。
// Fallbackは、Keyに対応するグループが未登録の場合や設計時プレビュー用の保険として使われる。
public class LocExtension : MarkupExtension
{
    public string? Key { get; set; }

    public object? Fallback { get; set; }

    public LocExtension()
    {
    }

    public LocExtension(string key)
    {
        Key = key;
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
        {
            return Fallback;
        }

        var binding = new Binding($"[{Key}]")
        {
            Source = LocalizationManager.Instance,
            Mode = BindingMode.OneWay,
            FallbackValue = Fallback,
            TargetNullValue = Fallback,
        };

        return binding.ProvideValue(serviceProvider);
    }
}
