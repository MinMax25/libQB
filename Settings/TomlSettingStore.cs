using System.IO;
using Tomlyn;

namespace libQB.Settings;

public sealed class TomlSettingStore<T>
    : ISettingStore<T>
    where T : class, new()
{
    private readonly string filePath;

    public TomlSettingStore(string filePath)
    {
        this.filePath = filePath;
    }

    public T Load()
    {
        if (File.Exists(filePath) == false)
            return new T();

        var tomlString = File.ReadAllText(filePath);
        return TomlSerializer.TryDeserialize<T>(tomlString, out var value, new TomlSerializerOptions())
            ? value!
            : new T();
    }

    public void Save(T settings)
    {
        var tomlString = TomlSerializer.Serialize(settings);
        File.WriteAllText(filePath, tomlString);
    }
}
