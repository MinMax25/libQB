using System.IO;

namespace libQB.Localization;

public sealed class LanguagePack
{
    public string Code { get; }

    public Dictionary<string, Dictionary<string, string>> Groups { get; } = new();

    private LanguagePack(string code)
    {
        Code = code;
    }

    public static LanguagePack LoadFromFolder(string code, string folderPath)
    {
        var pack = new LanguagePack(code);

        foreach (var file in Directory.GetFiles(folderPath, "*.resx"))
        {
            var group = Path.GetFileNameWithoutExtension(file);
            var table = ResxReader.Load(file);

            if (table.Count > 0)
            {
                pack.Groups[group] = table;
            }
        }

        return pack;
    }
}
