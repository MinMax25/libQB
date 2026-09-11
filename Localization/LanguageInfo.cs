namespace libQB.Localization;

public sealed class LanguageInfo
{
    // カルチャコード (例: "ja-JP", "zh-TW")
    public required string Code { get; init; }

    // 言語選択コンボボックス等に表示する名前
    public required string DisplayName { get; init; }

    // アセンブリに埋め込み済み(コンパイル時同梱)の言語かどうか
    public bool IsBuiltIn { get; init; }
}
