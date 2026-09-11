using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Resources;

namespace libQB.Localization;

// 組み込み言語(English/日本語)に加えて、外部フォルダに置かれた言語パックを
// 実行時に発見・読み込みし、キー単位で文字列の上書き解決を行う。
// インデクサ(this[fullKey])とPropertyChangedにより、言語切替をXAMLのBindingへ即時反映できる。
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

    // グループ名(例: "libQB.Menu")と、組み込みResxのResourceManagerの対応
    private readonly Dictionary<string, ResourceManager> _resourceManagers = new();

    // リソース値(例: "詳細")から fullKey ("グループ名.キー名") への逆引き辞書
    private readonly Dictionary<string, string> _valueToFullKeyMap = new();

    private LanguagePack? _activePack;

    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

    public IReadOnlyList<LanguageInfo> AvailableLanguages { get; private set; }

    // fullKey ("グループ名.キー名") やキー名、または x:Static 経由のリソース値を指定して、現在の言語での解決済み文字列を取得する。
    // XAMLからは {loc:Loc Key=...} 経由でこのインデクサにBindingされる。
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

            // ピリオドを含まない場合、または x:Static 評価後の文字列値の場合
            if (_valueToFullKeyMap.TryGetValue(keyOrValue, out var resolvedFullKey))
            {
                return this[resolvedFullKey];
            }

            // キー名だけで検索を試みる
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

    // 各プロジェクト固有の組み込みResx(例: "QBTemplate.Resources")をグループ名で登録する
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
            // 無視
        }
    }

    // 指定フォルダ配下のサブフォルダ(カルチャコード名)を言語パックとして読み込む
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

    // 現在有効な言語を切り替える(組み込み言語の場合は上書きパックなし=常にフォールバックを使用)
    // 切替後にPropertyChangedを発行し、Bindingしている全UI要素へ即時反映する。
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

        // インデクサ全体の再評価をWPFのBindingへ通知する(ObservableCollectionのIndexer通知と同じ手法)
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }

    // fullKey は "グループ名.キー名" (例: "libQB.Menu.File")
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

    // C#コードから利用する簡易ヘルパー。外部言語パックの上書き、無ければ組み込みResxを
    // 現在のカルチャで参照する。どちらにも無ければ登録漏れ・typoとみなし例外を投げる
    // (fallback文字列を重複定義させないための版)。
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
