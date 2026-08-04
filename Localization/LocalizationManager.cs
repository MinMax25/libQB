using System.ComponentModel;
using System.Globalization;
using System.IO;
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

    private LanguagePack? _activePack;

    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

    public IReadOnlyList<LanguageInfo> AvailableLanguages { get; private set; }

    // fullKey ("グループ名.キー名") を指定して、現在の言語での解決済み文字列を取得する。
    // XAMLからは {loc:Loc Key=...} 経由でこのインデクサにBindingされる。
    public string? this[string fullKey]
    {
        get
        {
            if (TryGetOverride(fullKey, out var value))
            {
                return value;
            }

            int idx = fullKey.LastIndexOf('.');
            if (idx <= 0 || idx == fullKey.Length - 1)
            {
                return null;
            }

            var group = fullKey[..idx];
            var key = fullKey[(idx + 1)..];

            if (_resourceManagers.TryGetValue(group, out var manager))
            {
                return manager.GetString(key, _currentCulture);
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

    // C#コードから利用する簡易ヘルパー。上書きが無ければfallbackをそのまま返す
    public string Resolve(string fullKey, string fallback)
    {
        return TryGetOverride(fullKey, out var value) ? value! : fallback;
    }
}
