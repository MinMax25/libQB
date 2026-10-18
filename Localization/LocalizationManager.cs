using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Resources;

namespace libQB.Localization;

public sealed class LocalizationManager : INotifyPropertyChanged
{
    public static LocalizationManager Instance { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly List<LanguageInfo> _builtIn =
    [
        new LanguageInfo { Code = "en-US", DisplayName = "English", IsBuiltIn = true },
        new LanguageInfo { Code = "ja-JP", DisplayName = "日本語", IsBuiltIn = true },
    ];

    private readonly Dictionary<string, LanguagePack> _externalPacks = new();

    private readonly Dictionary<string, ResourceManager> _resourceManagers = new();

    private readonly Dictionary<string, string> _valueToFullKeyMap = new();

    private LanguagePack? _activePack;

    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

    public IReadOnlyList<LanguageInfo> AvailableLanguages { get; private set; }

    public string? this[string keyOrValue]
    {
        get
        {
            if (string.IsNullOrEmpty(keyOrValue))
            {
                return null;
            }

            if (TryGetOverride(keyOrValue, out var value))
            {
                return value;
            }

            int idx = keyOrValue.LastIndexOf('.');
            if (idx > 0 && idx < keyOrValue.Length - 1)
            {
                var group = keyOrValue[..idx];
                var key = keyOrValue[(idx + 1)..];

                if (_resourceManagers.TryGetValue(group, out var manager))
                {
                    return manager.GetString(key, _currentCulture);
                }
            }

            if (_valueToFullKeyMap.TryGetValue(keyOrValue, out var resolvedFullKey))
            {
                return this[resolvedFullKey];
            }

            foreach (var (grp, mgr) in _resourceManagers)
            {
                var val = mgr.GetString(keyOrValue, _currentCulture);
                if (val != null)
                {
                    return val;
                }
            }

            return null;
        }
    }

    private LocalizationManager()
    {
        AvailableLanguages = _builtIn;
    }

    public void RegisterResourceManager(string group, ResourceManager manager)
    {
        _resourceManagers[group] = manager;
        BuildValueToFullKeyMap(group, manager);
    }

    private void BuildValueToFullKeyMap(string group, ResourceManager manager)
    {
        try
        {
            CultureInfo[] cultures = [CultureInfo.InvariantCulture, new CultureInfo("ja-JP"), CultureInfo.CurrentUICulture, CultureInfo.CurrentCulture];
            foreach (var culture in cultures.Distinct())
            {
                var set = manager.GetResourceSet(culture, true, true);
                if (set != null)
                {
                    foreach (System.Collections.DictionaryEntry entry in set)
                    {
                        if (entry.Key is string key)
                        {
                            var fullKey = $"{group}.{key}";
                            var val = manager.GetString(key, culture);
                            if (!string.IsNullOrEmpty(val))
                            {
                                _valueToFullKeyMap[val] = fullKey;
                            }
                        }
                    }
                }
            }
        }
        catch
        {
        }
    }

    public void Initialize(string languagesFolderPath)
    {
        _externalPacks.Clear();

        if (Directory.Exists(languagesFolderPath))
        {
            foreach (var dir in Directory.GetDirectories(languagesFolderPath))
            {
                var code = Path.GetFileName(dir);
                var pack = LanguagePack.LoadFromFolder(code, dir);

                if (pack.Groups.Count > 0)
                {
                    _externalPacks[code] = pack;
                }
            }
        }

        var combined = new List<LanguageInfo>(_builtIn);

        foreach (var code in _externalPacks.Keys)
        {
            string displayName;
            try
            {
                displayName = new CultureInfo(code).NativeName;
            }
            catch (CultureNotFoundException)
            {
                displayName = code;
            }

            combined.Add(new LanguageInfo { Code = code, DisplayName = displayName, IsBuiltIn = false });
        }

        AvailableLanguages = combined;
    }

    public void SetCulture(string cultureCode)
    {
        _activePack = _externalPacks.TryGetValue(cultureCode, out var pack) ? pack : null;

        try
        {
            _currentCulture = new CultureInfo(cultureCode);
        }
        catch (CultureNotFoundException)
        {
            _currentCulture = CultureInfo.CurrentUICulture;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }

    public bool TryGetOverride(string fullKey, out string? value)
    {
        value = null;

        if (_activePack == null || string.IsNullOrEmpty(fullKey))
        {
            return false;
        }

        int idx = fullKey.LastIndexOf('.');
        if (idx <= 0 || idx == fullKey.Length - 1)
        {
            return false;
        }

        var group = fullKey[..idx];
        var key = fullKey[(idx + 1)..];

        if (_activePack.Groups.TryGetValue(group, out var table) && table.TryGetValue(key, out var value2))
        {
            value = value2;
            return true;
        }

        return false;
    }

    private string Require(string fullKey)
    {
        if (TryGetOverride(fullKey, out var value))
        {
            return value!;
        }

        int idx = fullKey.LastIndexOf('.');
        if (idx > 0 && idx < fullKey.Length - 1)
        {
            var group = fullKey[..idx];
            var key = fullKey[(idx + 1)..];

            if (_resourceManagers.TryGetValue(group, out var manager))
            {
                var resolved = manager.GetString(key, _currentCulture);
                if (resolved is not null)
                {
                    return resolved;
                }
            }
        }

        throw new KeyNotFoundException($"Localization key not found: {fullKey}");
    }

    public string Require<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpr && memberExpr.Member.DeclaringType is Type declaringType)
        {
            var group = declaringType.FullName ?? declaringType.Name;
            var fullKey = $"{group}.{memberExpr.Member.Name}";
            return Require(fullKey);
        }
        throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
    }

    public string Require<TResource>(string key)
    {
        var group = typeof(TResource).FullName ?? typeof(TResource).Name;
        var fullKey = $"{group}.{key}";
        return Require(fullKey);
    }
}
