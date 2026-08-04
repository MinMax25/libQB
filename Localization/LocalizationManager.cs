using System.Globalization;
using System.IO;

namespace libQB.Localization;

// 組み込み言語(English/日本語)に加えて、外部フォルダに置かれた言語パックを
// 実行時に発見・読み込みし、キー単位で文字列の上書き解決を行う。
public sealed class LocalizationManager
{
    public static LocalizationManager Instance { get; } = new();

    private readonly List<LanguageInfo> _builtIn =
    [
        new LanguageInfo { Code = "en-US", DisplayName = "English", IsBuiltIn = true },
        new LanguageInfo { Code = "ja-JP", DisplayName = "日本語", IsBuiltIn = true },
    ];

    private readonly Dictionary<string, LanguagePack> _externalPacks = new();

    private LanguagePack? _activePack;

    public IReadOnlyList<LanguageInfo> AvailableLanguages { get; private set; }

    private LocalizationManager()
    {
        AvailableLanguages = _builtIn;
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
    public void SetCulture(string cultureCode)
    {
        _activePack = _externalPacks.TryGetValue(cultureCode, out var pack) ? pack : null;
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
