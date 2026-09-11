using System.Windows.Data;
using System.Windows.Markup;

namespace libQB.Localization;

// XAMLで {loc:Loc Key=libQB.Menu.File} のように使用する。
// LocalizationManagerのインデクサへBindingすることで、言語切替(SetCulture)のたびに
// PropertyChanged通知経由で表示中のテキストが自動的に再評価される。
// Fallbackが未指定の場合、LocalizationManagerまたはアセンブリ内のリソースから自動補完される。
public class LocExtension : MarkupExtension
{
    public string? Key { get; set; }

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
            return default;
        }

        var fallback = GetDefaultFallback(Key);

        var binding = new Binding($"[{Key}]")
        {
            Source = LocalizationManager.Instance,
            Mode = BindingMode.OneWay,
            FallbackValue = fallback,
            TargetNullValue = fallback,
        };

        return binding.ProvideValue(serviceProvider);
    }

    private static object? GetDefaultFallback(string fullKey)
    {
        var resolved = LocalizationManager.Instance[fullKey];
        if (resolved != null)
        {
            return resolved;
        }

        int idx = fullKey.LastIndexOf('.');
        if (idx > 0 && idx < fullKey.Length - 1)
        {
            var group = fullKey[..idx];
            var key = fullKey[(idx + 1)..];

            var fromAsm = TryResolveFromLoadedAssemblies(group, key);
            if (fromAsm != null)
            {
                return fromAsm;
            }

            return key;
        }

        return fullKey;
    }

    private static string? TryResolveFromLoadedAssemblies(string group, string key)
    {
        try
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = asm.GetType(group);
                if (type != null)
                {
                    var prop = type.GetProperty("ResourceManager", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (prop?.GetValue(null) is System.Resources.ResourceManager rm)
                    {
                        var str = rm.GetString(key);
                        if (str != null)
                        {
                            return str;
                        }
                    }
                }
            }
        }
        catch
        {
            // 失敗時は無視
        }
        return null;
    }
}
