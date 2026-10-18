namespace libQB.Settings;

public interface ISettingStore<T>
    where T : class, new()
{
    T Load();

    void Save(T settings);
}
