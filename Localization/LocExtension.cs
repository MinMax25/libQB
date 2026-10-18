using System.Windows.Data;
using System.Windows.Markup;

namespace libQB.Localization;

public class LocExtension : MarkupExtension
{
    public object? Key { get; set; }

    public LocExtension()
    {
    }

    public LocExtension(object key)
    {
        Key = key;
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        var keyStr = Key?.ToString();
        if (string.IsNullOrEmpty(keyStr))
        {
            return default;
        }

        var fallback = GetDefaultFallback(keyStr);

        var binding = new Binding($"[{keyStr}]")
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
        }
        return null;
    }
}
