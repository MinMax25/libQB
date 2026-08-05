# libQB

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet)](https://dotnet.microsoft.com/)
[![MahApps.Metro](https://img.shields.io/badge/MahApps.Metro-2.4.11-green)](https://github.com/MahApps/MahApps.Metro)
[![CommunityToolkit.Mvvm](https://img.shields.io/badge/MVVM%20Toolkit-8.4.2-orange)](https://github.com/CommunityToolkit/dotnet)

QBシリーズのWPFアプリケーションで共用するUI・アプリケーション基盤ライブラリです。
MahApps.MetroとCommunityToolkit.Mvvmを利用し、アプリ固有の機能から独立した共通サービスやWPF拡張を提供します。

現在のパッケージバージョンは `2.0.0`、Target Frameworkは `net10.0-windows7.0` です。

## v2.0.0へのアップグレード

v1.x.xからv2.0.0への更新には破壊的変更が含まれます。既存アプリケーションは、パッケージを更新する前に次の変更点を確認してください。

- libQB内蔵のresxと `libQB.Properties` 配下のリソースクラスを廃止しました。
- `LocalizationManager` はlibQBのResourceManagerを自動登録しなくなりました。必要なResourceManagerは利用側で `RegisterResourceManager` を使用して登録してください。
- 共通ダイアログとクリアボタンの文言は `DialogTextProvider` から取得します。ローカライズする場合は、利用側で各delegateを設定してください。

v1.x.xで `libQB.Properties.Dialog`、`libQB.Properties.Menu`、`libQB.Properties.Hamburger`、`libQB.Properties.Resources` を直接参照しているコードは、そのままではビルドできません。利用側のresxまたは `LocalizationManager`／`DialogTextProvider` を使用する実装へ移行してください。

## 主な機能

- MahApps.Metro上の共通メッセージダイアログ
- Windows標準のファイル／フォルダー選択ダイアログ
- DIからViewModelを解決するモーダルWindowサービス
- Undo/Redoの履歴管理とプロパティ変更アクション
- ResourceManagerと外部resx言語パックを統合するLocalization基盤
- TOML形式の汎用設定ストア
- WPF Behavior、添付プロパティ、値コンバーター
- DI登録意図を表す属性メタデータ
- Light/Darkテーマを表す共通Enum

DIコンテナー、属性のスキャン処理、画面遷移、アプリ固有の設定モデルは含みません。これらは利用側アプリケーションで構成します。

## 必要環境

- .NET SDK 10.0
- WPFを利用できるWindows環境

主なNuGet依存パッケージは次のとおりです。

| パッケージ | バージョン |
|---|---:|
| CommunityToolkit.Mvvm | 8.4.2 |
| MahApps.Metro | 2.4.11 |
| MahApps.Metro.IconPacks.FontAwesome | 6.2.1 |
| MahApps.Metro.IconPacks.Material | 6.2.1 |
| Tomlyn | 2.10.1 |

## ビルド

```powershell
dotnet restore libQB.sln
dotnet build libQB.sln
```

ビルド時にNuGetパッケージも生成されます。

## ドキュメント

- [概要](docs/Overview.md)
- [アーキテクチャ](docs/Architecture.md)
- [公開型リファレンス](docs/ClassReference.md)

## ライセンス

このプロジェクトはMIT Licenseで提供されます。詳細は [LICENSE.txt](LICENSE.txt) を参照してください。
