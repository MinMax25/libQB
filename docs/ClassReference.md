# libQB リファレンス

更新者：Codex
最終更新日時：2026-09-12 14:50

この文書は、libQB 2.0.0の主要な公開型と利用上の契約を機能別にまとめたものです。

## DI属性（`libQB.Attributes`）

| 型 | 概要 |
| --- | --- |
| `DIAttribute` | `Type1`、`Type2` を保持する属性基底クラス |
| `DIWindowAttribute<TWindow>` | Window型の登録意図を表す属性 |
| `DIWindowAttribute<TInterface,TWindow>` | 契約型とWindow型の対応を表す属性 |
| `DIPageAttribute<T>` | Page型の登録意図を表す属性 |
| `DIUserControlAttribute<T>` | UserControl型の登録意図を表す属性 |
| `DISingletonAttribute` | 実装型自身をSingletonとして扱う意図を表す属性 |
| `DISingletonAttribute<T>` | 契約型に対応するSingletonの意図を表す属性 |

これらの属性はメタデータのみを提供します。リフレクションによる検索とDIコンテナーへの登録は利用側で実装してください。

## ダイアログ（`libQB.DialogServices`）

### `IDialogService`

ファイル選択と共通メッセージダイアログの契約です。

| メソッド | 概要 |
| --- | --- |
| `ShowOpenFileDialog` | ファイルを開くダイアログを表示 |
| `ShowSaveFileDialog` | ファイルを保存するダイアログを表示 |
| `ShowSelectFolderDialog` | フォルダー選択ダイアログを表示 |
| `ShowGenericDialogAsync` | 指定したOptionsで共通ダイアログを表示 |
| `ShowInformationAsync` | 情報ダイアログを表示 |
| `ShowAlertAsync` | アラートダイアログを表示 |
| `ShowErrorAsync` | エラーダイアログを表示 |
| `ShowWarningAsync` | 警告ダイアログを表示 |
| `ShowConfirmAsync` | 確認ダイアログを表示 |

ファイル／フォルダーダイアログはキャンセル時にnullを返します。共通ダイアログはMainWindowが `MetroWindow` でない場合にnullを返します。

### ダイアログ構成型

| 型 | 概要 |
| --- | --- |
| `DialogService` | `IDialogService` のWPF/MahApps実装 |
| `DialogType` | `Information`、`Alert`、`Warning`、`Error`、`Confirm` を表すEnum |
| `GenericDialogOptions` | タイトル、本文、アイコン、色、ボタン表示を保持 |
| `GenericDialogViewModel` | 共通ダイアログの表示状態とCommandを提供 |
| `GenericDialog` | `BaseMetroDialog` 派生の共通ダイアログView |
| `DialogTextProvider` | ボタン、タイトル、tooltip文言を返すdelegateの静的な差し込み口 |

`DialogTextProvider` の既定値は英語です。言語切替へ追従させる場合は、アプリケーション起動時に各 `Func<string>` を設定してください。

## Windowサービス（`libQB.WindowServices`）

### `IWindowService` / `WindowService`

| メンバー | 概要 |
| --- | --- |
| `ShowWindow<TWindow,TViewModel>` | ViewModelをDIから解決してWindowをモーダル表示 |
| `ShowWindowWithCallback<TWindow,TViewModel,TResult>` | パラメーターを渡して表示し、終了後に結果を通知 |
| `CloseWindow` | 指定したWindowを閉じる |
| `WindowService.Owner` | 公開Ownerプロパティ。現在の表示処理はメソッド引数のownerを使用 |

Window型には `Window, new()`、ViewModel型には `class` の制約があります。Windowは `new TWindow()` で生成され、ViewModelのみが `IServiceProvider` から解決されます。

### 補助インターフェース

| 型 | メソッド | 用途 |
| --- | --- | --- |
| `IParameterReceiver` | `ReceiveParameter(object)` | callback版の表示前に入力を受け取る |
| `IResultProvider<TResult>` | `GetResult()` | Window終了後に結果を返す |
| `IWindowClosingAware` | `OnWindowClosingAsync()` | ViewModelが終了可否を返すための契約 |

`IWindowClosingAware` の呼び出しは利用側Windowで実装してください。

## Undo/Redo（`libQB.UndoRedo`）

### `IUndoManager` / `UndoManager`

| メンバー | 概要 |
| --- | --- |
| `CanUndo` | Undo可能かを返す |
| `CanRedo` | Redo可能かを返す |
| `IsExecutingUndoRedo` | Undo/Redo実行中かを返す |
| `PushAction` | 履歴へActionを追加し、Redo履歴を破棄 |
| `RegisterPropertyChange<T>` | プロパティ変更を履歴へ追加 |
| `Undo` | 直前のActionを取り消す |
| `Redo` | 取り消したActionを再実行 |
| `Clear` | 全履歴を破棄 |

`CanUndo` と `CanRedo` の変更は `INotifyPropertyChanged` で通知されます。履歴上限、永続化、スレッド同期はありません。

### Action型

| 型 | 概要 |
| --- | --- |
| `IUndoableAction` | `Undo()` と `Redo()` を定義する契約 |
| `PropertyChangeAction<T>` | 式木で指定したプロパティへold/new値を設定 |
| `CompoundUndoableAction` | 複数Actionを1つの履歴単位として管理 |
| `UndoManagerExtensions` | Actionを履歴へ追加する `Do` 拡張メソッド |

`UndoManagerExtensions.Do` はActionを即時実行せず、履歴へ追加します。

## Localization（`libQB.Localization`）

### `LocalizationManager`

プロセス内Singletonの `Instance` を通じて、言語一覧、ResourceManager、外部言語パック、現在のカルチャを管理します。

| メンバー | 概要 |
| --- | --- |
| `AvailableLanguages` | 利用可能な言語情報の一覧 |
| `this[string fullKey]` | 外部パック、ResourceManagerの順に文字列を解決 |
| `RegisterResourceManager` | group名とResourceManagerを登録 |
| `Initialize` | カルチャ別サブフォルダーから外部resxを読み込む |
| `SetCulture` | 現在のカルチャと外部パックを切り替える |
| `TryGetOverride` | 外部パックの値だけを検索 |
| `Resolve` | 外部パックになければ指定fallbackを返す |

full keyは最後の `.` でgroup名とkey名に分割されます。たとえば `MyApp.Resources.Save` は `MyApp.Resources` と `Save` に分割されます。

libQB自身はResourceManagerを登録しません。`Initialize` によるディレクトリ列挙やresx解析で発生した例外は呼び出し元へ伝播します。

### 補助型

| 型 | 概要 |
| --- | --- |
| `LanguageInfo` | `Code`、`DisplayName`、`IsBuiltIn` を持つ言語情報 |
| `LanguagePack` | 1カルチャ分のgroup/key/valueを保持 |
| `ResxReader` | resxの文字列data要素を辞書として読み込む |
| `LocExtension` | `LocalizationManager` のインデクサへOneWay BindingするMarkupExtension |

`LocExtension` は `Key` と `Fallback` を受け取り、`SetCulture` 時の通知で表示を再評価します。`LocalizationManager` には、ResourceManagerまたは外部パックから必須文字列を取得する `Require<TResource>(key)` とプロパティ式版の `Require` もあります。

## Settings（`libQB.Settings`）

### `ISettingStore<T>`

`T : class, new()` を満たす設定モデルに対して、`Load()` と `Save(T settings)` を定義します。

### `TomlSettingStore<T>`

Tomlynを利用する `ISettingStore<T>` 実装です。コンストラクターで保存先ファイルパスを指定します。

- ファイルが存在しない場合は `new T()` を返します。
- デシリアライズに失敗した場合は `new T()` を返します。
- enum等の変換規則は設定型側で指定します。
- 保存先ディレクトリの作成と例外変換は行いません。

## WPF Behavior／添付機能（`libQB.Behaviors`）

| 型 | 概要 |
| --- | --- |
| `ClearableComboBox` | ComboBoxへクリアボタンを追加する添付機能 |
| `ClosePopupOnOutsideClickBehavior` | Popup外のクリックでPopupを閉じるBehavior |
| `ContextMenuBehavior` | ContextMenuの表示可否を制御するBehavior |
| `DataGridExtendBehavior` | 編集状態、選択行、Delete、SortをViewModelへ接続 |
| `FocusValidationBlockBehavior` | 検証エラー時のフォーカス移動を抑止する添付機能 |

`DataGridExtendBehavior` の主なDependencyPropertyは `IsEditing`、`IsSorted`、`SelectedRows`、`SelectedRow`、`DeleteCommand`、`SelectionChangedCommand` です。

## Value Converter（`libQB.ValueConverters`）

| 型 | Convert | ConvertBack |
| --- | --- | --- |
| `BaseThemeConverter` | 値とparameterの文字列表現を比較 | trueの場合にparameterをtarget Enumへ変換。falseの場合は `Binding.DoNothing` |
| `IncrementConverter` | 整数値へ1を加算 | 未実装 |
| `InverseBoolConverter` | boolを反転 | boolを反転 |
| `TrimConverter` | 値を文字列化 | 文字列化して前後の空白を除去 |

未対応の値に対する戻り値や例外は各Converterで異なります。Bindingへ設定する値の型を合わせてください。

## Enum（`libQB.Enums`）

### `BaseTheme`

アプリケーションのベーステーマを表すEnumです。

- `Light`
- `Dark`

JSONでは文字列Enumとして扱われます。テーマを実際に適用する処理は利用側で実装してください。
