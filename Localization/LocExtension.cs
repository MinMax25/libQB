using System.Windows.Markup;

namespace libQB.Localization;

// XAMLで {loc:Loc Key=libQB.Menu.File, Fallback={x:Static reslibQB:Menu.File}} のように使用する。
// 外部言語パックにKeyの上書きがあればそれを、無ければFallback(従来通りのx:Static参照)を返す。
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

        return LocalizationManager.Instance.TryGetOverride(Key, out var value) ? value : Fallback;
    }
}
