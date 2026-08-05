# libQB 概要

## libQBとは

libQBは、Windows向けWPFアプリケーションで利用できるUI・アプリケーション基盤ライブラリです。アプリケーション固有の画面やモデルには依存せず、複数のアプリケーションで再利用しやすい共通機能を提供します。

## 動作環境

| 項目 | 値 |
|---|---|
| パッケージバージョン | `2.0.0` |
| Target Framework | `net10.0-windows7.0` |
| UI Framework | WPF |
| Nullable参照型 | 有効 |
| 対応Platform | `AnyCPU`、`x64` |
| ライセンス | MIT |

## 提供機能

| 名前空間 | 機能 |
|---|---|
| `libQB.Attributes` | DI登録対象を表す属性メタデータ |
| `libQB.Behaviors` | ComboBox、Popup、ContextMenu、DataGrid、入力検証向けのWPF拡張 |
| `libQB.DialogServices` | MahApps.MetroダイアログとWindows標準ファイルダイアログ |
| `libQB.Enums` | Light/Darkテーマを表す共通列挙型 |
| `libQB.Localization` | ResourceManagerと外部resx言語パックを統合する文字列解決 |
| `libQB.Settings` | 汎用設定ストアとTOML実装 |
| `libQB.UndoRedo` | Undo/Redo履歴と変更アクション |
| `libQB.ValueConverters` | WPF Binding向け値コンバーター |
| `libQB.WindowServices` | ViewModelをDIから解決するモーダルWindowサービス |

## 主な依存パッケージ

| パッケージ | バージョン | 用途 |
|---|---:|---|
| `CommunityToolkit.Mvvm` | 8.4.2 | Observable propertyとRelayCommandの生成 |
| `MahApps.Metro` | 2.4.11 | MetroWindow上のカスタムダイアログ |
| `MahApps.Metro.IconPacks.FontAwesome` | 6.2.1 | アイコンパック |
| `MahApps.Metro.IconPacks.Material` | 6.2.1 | 共通ダイアログのアイコン |
| `Tomlyn` | 2.10.1 | TOMLのシリアライズとデシリアライズ |

## 利用側で構成するもの

libQBは次の機能を提供しません。必要に応じて利用側アプリケーションで構成してください。

- DIコンテナーと属性スキャン処理
- ナビゲーションと画面ライフサイクル
- アプリケーション固有の設定モデル、保存先、保存タイミング
- 言語選択の保存とResourceManagerの登録
- テーマの適用処理
- ロギング、通信、データベース、ドメインモデル

## 利用上の注意

- MahAppsダイアログを表示するには、`Application.Current.MainWindow` が `MetroWindow` である必要があります。
- ファイル／フォルダーダイアログは、キャンセル時にnullを返します。
- `WindowService` はWindowを生成し、ViewModelのみを `IServiceProvider` から解決します。
- `LocalizationManager` はlibQB自身のResourceManagerを自動登録しません。利用側のResourceManagerは `RegisterResourceManager` で登録してください。
- `DialogTextProvider` の既定文言は英語です。ローカライズする場合は、利用側で各delegateを設定してください。
- `TomlSettingStore<T>` は保存先ディレクトリを作成しません。保存前に利用側で用意してください。
- Undo/Redo履歴には件数制限と永続化機能がありません。

## 関連文書

- [アーキテクチャ](Architecture.md)
- [公開型リファレンス](ClassReference.md)
