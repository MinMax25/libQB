# libQB 公開型リファレンス

## 1. 調査範囲と利用箇所の表記

2026-08-04時点のソースに宣言された公開型を機能別に記載する。「利用箇所」はlibQB内部と、参照可能だったQBTemplate、QBDrumMap、QBMidicon、QBChordBook、QBConference、QBLipsync、QBPigeonDocのC#／XAMLを検索した結果である。型名が外部コードに直接現れない場合は「現リポジトリでは参照を確認できない」とした。公開メンバーの網羅的API仕様ではなく、責務と主要な使い方を理解するためのリファレンスである。

## 2. DI属性（`libQB.Attributes`）

配置はすべて `Attributes/DIAttributes.cs`、公開範囲は `public` である。

### `DIAttribute`

- 種別: 抽象属性基底クラス
- 主責務: DI登録対象に最大2個の `Type` メタデータを付加する。
- 主プロパティ: `Type1`、`Type2`（いずれも読み書き可能・nullable）
- 主メソッド: コンストラクター `DIAttribute(Type? value1 = null, Type? value2 = null)`
- 関連型: すべてのDI属性派生型
- 利用箇所: QBTemplate、QBDrumMap、QBMidicon、QBChordBook、QBConference、QBLipsync、QBPigeonDocの属性スキャン。
- 注意: 登録処理は提供しない。アプリ側がリフレクションで属性と `Type1` / `Type2` を解釈する。

### DI属性派生型

| 名前 | 種別・型制約 | 格納する型 | 確認した利用箇所 | 注意 |
|---|---|---|---|---|
| `DIWindowAttribute<TWindow>` | 属性、`TWindow : Window` | `Type1=TWindow` | 上記QB各プロジェクト | Window型を1個表す |
| `DIWindowAttribute<TInterface,TWindow>` | 属性、interface側はclass、実装側はWindow | `Type1=TInterface`, `Type2=TWindow` | 上記QB各プロジェクト | 型引数名はinterfaceだが、制約は `class` のみ |
| `DIPageAttribute<T>` | 属性、`T : Page` | `Type1=T` | 上記QB各プロジェクト | Page登録用メタデータ |
| `DIUserControlAttribute<T>` | 属性、`T : UserControl` | `Type1=T` | 上記QB各プロジェクト | UserControl登録用メタデータ |
| `DISingletonAttribute` | 属性 | なし | libQBのサービス、上記QB各プロジェクト | 実装型自身をSingleton扱いする意図を示す |
| `DISingletonAttribute<T>` | 属性 | `Type1=T` | `DialogService`、`WindowService`、`UndoManager`、上記QB各プロジェクト | 実装型との対応付けはスキャナー側の責務 |

## 3. ダイアログ（`libQB.DialogServices`）

### `IDialogService`

- 種別／配置: interface、`DialogServices/IDialogService.cs`
- 主責務: OSファイル選択と共通メッセージダイアログの契約。
- 主メソッド: `ShowOpenFileDialog`、`ShowSaveFileDialog`、`ShowSelectFolderDialog`、`ShowGenericDialogAsync`、`ShowInformationAsync`、`ShowAlertAsync`、`ShowErrorAsync`、`ShowWarningAsync`、`ShowConfirmAsync`
- 関連型: `DialogService`、`GenericDialogOptions`
- 利用箇所: QBTemplate、QBDrumMap、QBMidicon、QBConference、QBLipsync。
- 注意: ファイル系メソッドはキャンセル時に実質nullを返すが、戻り型は `Task<string>`。

### `DialogService`

- 種別／配置: class、`DialogServices/DialogService.cs`
- 主責務: `IDialogService` のWPF/MahApps実装。Windows標準ファイルダイアログを開き、共通ダイアログをMainWindowのMetroオーバーレイに表示する。
- 公開メソッド: `IDialogService` と同じ。
- 関連型: `MetroWindow`、`GenericDialog`、`GenericDialogViewModel`、`GenericDialogOptions`
- 利用箇所: 上記アプリで `IDialogService` 実装としてDI登録。libQB内部では `[DISingleton<IDialogService>]` を付与。
- 注意: MainWindowが `MetroWindow` でなければ共通ダイアログを表示せずnullを返す。ファイル系メソッドは非同期I/Oではなく同期ダイアログの結果を `Task.FromResult` で包む。

### ダイアログ構成型

| 名前 | 種別／配置 | 主責務・主要メンバー | 利用箇所と注意 |
|---|---|---|---|
| `DialogType` | enum、`DialogType.cs` | `Information`, `Alert`, `Warning`, `Error`, `Confirm` | `GenericDialogOptions.Create` 内部でプリセット選択に使用。外部での直接参照は現リポジトリでは確認できない |
| `GenericDialogOptions` | class、`GenericDialogOptions.cs` | Title、Message、Material Icon、Brush、ボタン文言、キャンセル表示を保持。`Create(DialogType, ...)` でプリセット生成 | `DialogService` 内部で利用。外部での直接参照は現リポジトリでは確認できない |
| `GenericDialogViewModel` | `ObservableObject` 派生partial class、`GenericDialogViewModel.cs` | 画面表示プロパティ、生成される `OkCommand` / `CancelCommand`、`CompletionSource` | `DialogService` / `GenericDialog.xaml` 内部。外部参照なし。コマンドはtrue/falseを一度だけ完了通知する |
| `GenericDialog` | `BaseMetroDialog` 派生partial class、`GenericDialog.xaml(.cs)` | 共通メッセージ画面 | `DialogService` 内部。外部参照なし |
| `DialogTextProvider` | static class、`DialogTextProvider.cs` | OK/Cancel/Yes/No、各タイトル、Clear tooltipを返す `Func<string>` プロパティ | libQB内部とQBTemplate。プロセス全体で可変な静的差し込み口であり、アプリ起動時の設定を想定 |

## 4. Windowサービス（`libQB.WindowServices`）

### `IWindowService` / `WindowService`

| 項目 | `IWindowService` | `WindowService` |
|---|---|---|
| 種別・配置 | interface、`IWindowService.cs` | class、`WindowService.cs` |
| 主責務 | Window表示・結果返却・Closeの契約 | Windowを生成し、ViewModelを `IServiceProvider` から解決してモーダル表示 |
| 主メソッド | `ShowWindow<TWindow,TViewModel>`、`ShowWindowWithCallback<TWindow,TViewModel,TResult>`、`CloseWindow` | 同左 |
| 主プロパティ | なし | `Owner`（現在の表示処理では参照されない） |
| 利用箇所 | QBTemplate、QBDrumMap、QBMidicon、QBConference、QBLipsync | 同アプリでDI登録 |

`ShowWindowWithCallback` はViewModelが `IParameterReceiver` なら表示前にパラメーターを渡し、Windowの `Closed` 後に `IResultProvider<TResult>` から結果を取得する。`ShowWindow` は引数 `parameter` を使用しない。両メソッドともWindowはDIではなく `new TWindow()` で生成し、`ShowDialog()` を呼ぶ。

### 補助インターフェース

| 名前 | 配置 | 主メソッド | 利用箇所 | 注意 |
|---|---|---|---|---|
| `IParameterReceiver` | `IParameterReceiver.cs` | `ReceiveParameter(object)` | QBDrumMap、QBMidicon | `ShowWindowWithCallback` が表示前に呼ぶ |
| `IResultProvider<TResult>` | `IResultProvider.cs` | `GetResult()` | QBDrumMap、QBMidicon、QBConference | WindowのClosedイベント後に結果を返す |
| `IWindowClosingAware` | `IWindowClosingAware.cs` | `Task<bool> OnWindowClosingAsync()` | QBDrumMap、QBMidicon | libQB内からは呼ばれない。ShellWindow等のアプリ側コードが終了可否確認に使う |

## 5. Undo/Redo（`libQB.UndoRedo`）

### `IUndoManager` / `UndoManager`

- 種別／配置: interface `IUndoManager.cs`、class `UndoManager.cs`
- 主責務: Undo/Redo用の2スタックを管理し、実行可否を通知する。
- 主プロパティ: `CanUndo`、`CanRedo`、`IsExecutingUndoRedo`
- 主メソッド: `PushAction`、`RegisterPropertyChange<T>`、`Undo`、`Redo`、`Clear`
- 関連型: `IUndoableAction`、`PropertyChangeAction<T>`
- 利用箇所: QBDrumMap、QBMidicon、QBLipsync。`UndoManager` は `[DISingleton<IUndoManager>]`。
- 注意: 履歴上限、スレッド同期、永続化はない。Undo/Redo実行中だけ再登録を抑止する。状態通知は `CanUndo` と `CanRedo` に対して発行され、`IsExecutingUndoRedo` のPropertyChangedは発行されない。

### アクション型

| 名前 | 種別／配置 | 主責務・メンバー | 利用箇所・注意 |
|---|---|---|---|
| `IUndoableAction` | interface、`IUndoableAction.cs` | `Undo()`、`Redo()` | libQB内部。外部で型名の直接参照は確認できない |
| `PropertyChangeAction<T>` | class、`PropertyChangeAction.cs` | 式木からsetterを構築し、old/new値を設定 | QBDrumMap。式のBodyは直接の `MemberExpression` が必要 |
| `CompoundUndoableAction` | class、`CompoundUndoableAction.cs` | `Add`、逆順Undo、順方向Redo、`IsEmpty` | QBDrumMap。空でもPush自体は禁止されない |
| `UndoManagerExtensions` | static class、`UndoManager.cs` | `Do(this IUndoManager, IUndoableAction)` | 現リポジトリでは参照を確認できない。名前に反してactionの実行はせずPushのみ |

## 6. Localization（`libQB.Localization`）

### `LocalizationManager`

- 種別／配置: sealed class、`Localization/LocalizationManager.cs`
- 公開範囲: public、private constructorを持つプロセス内Singleton（`Instance`）
- 主責務: ResourceManager登録、外部言語パック探索、カルチャ切替、キー解決、WPF Bindingへの再評価通知。
- 主プロパティ: `Instance`、`AvailableLanguages`、文字列インデクサ `this[string fullKey]`
- 主メソッド: `RegisterResourceManager`、`Initialize`、`SetCulture`、`TryGetOverride`、`Resolve`
- 関連型: `LanguageInfo`、`LanguagePack`、`LocExtension`、`ResourceManager`
- 利用箇所: QBTemplate。libQB内部では `LocExtension` が参照。
- 注意: キーは最後の `.` でgroup/keyに分割する。外部パックの値が最優先で、なければ登録済みResourceManagerへフォールバックする。`Initialize` はディレクトリ列挙・resx解析例外を捕捉しない。`SetCulture` は無効なカルチャを現在UIカルチャへ戻すが、外部パック選択自体は辞書キーで先に決まる。

### Localization補助型

| 名前 | 種別／配置 | 主責務・主要メンバー | 利用箇所・注意 |
|---|---|---|---|
| `LanguageInfo` | sealed DTO、`LanguageInfo.cs` | required initの `Code`, `DisplayName` と `IsBuiltIn` | QBTemplate。`IsBuiltIn` はlibQB内では設定のみ |
| `LanguagePack` | sealed class、`LanguagePack.cs` | 1言語の `Code` とgroup/key/value辞書。`LoadFromFolder` | `LocalizationManager` 内部。外部直接参照なし。直下の `*.resx` のみ読む |
| `ResxReader` | static class、`ResxReader.cs` | `Load(path)` で文字列data要素を辞書化 | `LanguagePack` 内部。外部直接参照なし。非文字列resx機能は扱わない |
| `LocExtension` | `MarkupExtension` 派生class、`LocExtension.cs` | `Key`、`Fallback`、`ProvideValue` | QBTemplateのXAMLで `{loc:Loc ...}` として利用（Markup Extensionでは`Extension`接尾辞を省略）。ManagerのインデクサへのOneWay Bindingを返し、`Item[]` 通知で再評価される |

## 7. Settings（`libQB.Settings`）

### `ISettingStore<T>`

- 種別／配置: generic interface、`Settings/ISettingStore.cs`、`T : class, new()`
- 主責務: 設定モデルのロード／保存を永続化方式から分離する。
- 主メソッド: `T Load()`、`void Save(T settings)`
- 利用箇所: libQB内部、QBTemplate。

### `TomlSettingStore<T>`

- 種別／配置: sealed generic class、`Settings/TomlSettingStore.cs`
- 主責務: Tomlynによる `ISettingStore<T>` 実装。
- 主メソッド: constructor（保存先パス）、`Load`、`Save`
- 利用箇所: QBTemplate。
- 注意: ファイル不存在・`TryDeserialize`失敗は `new T()`。ディレクトリ作成、同時書込み制御、原子的置換、例外の変換は行わない。

## 8. WPF Behavior／添付機能（`libQB.Behaviors`）

| 名前 | 種別／配置 | 主責務・主プロパティ | 利用箇所 | 注意 |
|---|---|---|---|---|
| `ClearableComboBox` | static添付機能、`ClearableComboBox.cs` | `EnableClearButton`、`ClearValue` | QBDrumMap | テンプレートの最初のGridへボタンを動的追加。`ClearValue` は処理で未使用。選択ありならボタン表示 |
| `ClosePopupOnOutsideClickBehavior` | `Behavior<Popup>`、同名ファイル | MainWindow上のPopup外クリックを検出して閉じる | 現リポジトリでは参照を確認できない | `Application.Current.MainWindow` のPreviewMouseDownに購読。Popupが開いたままDetachされる場合のMainWindow購読解除は実装されていない |
| `ContextMenuBehavior` | `Behavior<FrameworkElement>` | `IsContextMenuEnabled` でOpeningを抑止 | QBDrumMap | ContextMenu自体の生成・状態変更は行わない |
| `DataGridExtendBehavior` | `Behavior<DataGrid>` | `IsEditing`, `IsSorted`, `SelectedRows`, `SelectedRow`, `DeleteCommand`, `SelectionChangedCommand` | QBDrumMap、QBMidicon | Enter確定、Delete転送、選択同期、3状態ソート。SelectionChangedCommandへ選択数を渡す |
| `FocusValidationBlockBehavior` | static添付機能 | `IsEnabled` | 現リポジトリでは有効な参照を確認できない（QBDrumMap、QBMidiconにコメントアウトされた設定例のみ存在） | DataContextが `INotifyDataErrorInfo` で、対象要素のTextBox.Text Bindingにエラーがある場合のみフォーカス移動を抑止 |

## 9. Value Converter（`libQB.ValueConverters`）

すべてpublic classで `IValueConverter` を実装する。

| 名前／配置 | Convert | ConvertBack | 利用箇所・注意 |
|---|---|---|---|
| `BaseThemeConverter.cs` | 値とparameterの文字列表現が等しいか | true時にtargetTypeのEnumをparse、それ以外 `Binding.DoNothing` | QBTemplate、QBDrumMap、QBMidicon、QBConference、QBLipsync |
| `IncrementConverter.cs` | `int.Parse($"0{value}") + 1` | `NotImplementedException` | QBMidicon。数値化できない非空値は例外 |
| `InverseBoolConverter.cs` | bool反転 | bool反転 | QBDrumMap、QBMidicon、QBLipsync。bool以外は `UnsetValue` |
| `TrimConverter.cs` | `ToString()` | `ToString()?.Trim()` | QBDrumMap、QBMidicon、QBLipsync |

## 10. Enum（`libQB.Enums`）

### `BaseTheme`

- 種別／配置: enum、`Enums/BaseTheme.cs`
- 値: `Light`, `Dark`
- 主責務: QBアプリ共通のベーステーマ表現。
- 関連型: `JsonStringEnumConverter<BaseTheme>` 属性、`BaseThemeConverter`
- 利用箇所: QBTemplate、QBDrumMap、QBMidicon、QBConference、QBLipsync。
- 注意: libQB自身はテーマ変更処理を提供しない。
