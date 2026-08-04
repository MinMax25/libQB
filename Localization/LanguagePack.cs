using System.IO;

namespace libQB.Localization;

// 外部フォルダから読み込んだ1言語分の文字列テーブル一式
// Groups のキーはファイル名(拡張子除く)。例: "libQB.Menu", "QBTheater.Resources"
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
