namespace libQB.Localization;

public sealed class LanguageInfo
{
    public required string Code { get; init; }

    public required string DisplayName { get; init; }

    public bool IsBuiltIn { get; init; }
}
