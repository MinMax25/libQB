using System.Xml.Linq;

namespace libQB.Localization;

// resxファイルからname/valueのペアのみを取り出す簡易リーダー
// (バイナリ埋め込み等は扱わず、文字列リソースの読み込みに特化)
public static class ResxReader
{
    public static Dictionary<string, string> Load(string path)
    {
        var result = new Dictionary<string, string>();

        var doc = XDocument.Load(path);
        var root = doc.Root;
        if (root == null)
        {
            return result;
        }

        foreach (var data in root.Elements("data"))
        {
            var name = (string?)data.Attribute("name");
            var value = data.Element("value")?.Value;

            if (!string.IsNullOrEmpty(name) && value != null)
            {
                result[name] = value;
            }
        }

        return result;
    }
}
