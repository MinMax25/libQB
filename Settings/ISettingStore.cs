namespace libQB.Settings;

// 汎用設定ストア。永続化方式(TOML等)への依存を持たない
public interface ISettingStore<T>
    where T : class, new()
{
    // 設定を読み込む。ファイルが存在しない、または読込に失敗した場合は既定値(new T())を返す
    T Load();

    // 設定を保存する。書込みに失敗した場合は例外がそのまま呼び出し元へ伝播する
    void Save(T settings);
}
