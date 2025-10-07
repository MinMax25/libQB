# libQB

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/)
[![MahApps.Metro](https://img.shields.io/badge/MahApps.Metro-2.4%2B-green)](https://github.com/MahApps/MahApps.Metro)
[![CommunityToolkit.Mvvm](https://img.shields.io/badge/MVVM%20Toolkit-8.4%2B-orange)](https://github.com/CommunityToolkit/dotnet)

WPF アプリケーション「QB」向けに開発された汎用ライブラリです。  
`CommunityToolkit.Mvvm` と `MahApps.Metro` をベースに、MVVM構造の補助、ダイアログ制御、Undo/Redo 機構などを提供します。

---

## 🌟 主な機能

### 🧩 MVVM支援
- `CommunityToolkit.Mvvm` に基づく ViewModel パターンを簡素化  
- `RelayCommand` や `ObservableProperty` の活用を前提としたヘルパー群を提供

### 🪟 ダイアログ／ウィンドウサービス
- MVVMに準拠したダイアログ表示を実現  
- `IWindowService`, `IParameterReceiver`, `IResultProvider<TResult>` により  
  ViewModel間のパラメータ受け渡しと結果取得を統一的に管理

### ♻️ Undo / Redo 機構
- `IUndoableAction` インターフェイスに基づく汎用的なアクション管理  
- `PropertyChangeAction<T>` によるプロパティ変更のUndo対応  
- `CompoundUndoableAction` による複数操作の一括トランザクション管理

### ⚙️ データバインディング支援
- `ObjectDataProvider` を利用した静的データのXAMLリソース展開をサポート  
- `CubaseScore.InstrumentsComboSource` などを簡単にバインド可能

### 🎛️ MahApps.Metro 拡張
- Metroスタイルダイアログ／メニュー構成の拡張  
- `HamburgerMenu`, `Dialog`, `Menu` のリソースを含む  
- リソース辞書によるテーマ管理・ローカライズ対応

---

## 🧱 プロジェクト構成（例）

```text
libQB/
├─ libQB.UndoRedo/          ... Undo/Redo 機構
├─ libQB.IO/                ... XML / JSON 読み書きヘルパー
├─ libQB.Common/            ... 共通ユーティリティ群
├─ libQB.Audio/             ... オーディオメータリング・再生補助
├─ Properties/              ... WPF用リソース
└─ libQB.csproj
```

---

## 🚀 インストール

NuGet パッケージとして利用可能です。

```bash
dotnet add package libQB
```

または Visual Studio の  
**[NuGet パッケージの管理] → 「libQB」を検索 → インストール**

---

## 🧩 使用例

### ダイアログの表示（MVVM対応）

```csharp
public class MainViewModel
{
    private readonly IWindowService _windowService;

    public MainViewModel(IWindowService windowService)
    {
        _windowService = windowService;
    }

    [RelayCommand]
    private void OpenDialog()
    {
        _windowService.ShowWindowWithCallback<MyDialogViewModel, DialogResult>(
            parameter: new DialogParameter { Title = "設定" },
            callback: result => 
            {
                // 結果の処理
            });
    }
}
```

### Undo / Redo の実装例

```csharp
var history = new UndoHistory();
history.Execute(new PropertyChangeAction<MyViewModel>(
    target, nameof(target.Property), oldValue, newValue));
```

### ObjectDataProvider による XAML バインド例

```xml
<ObjectDataProvider x:Key="InstrumentsSource"
                    ObjectType="{x:Type local:CubaseScore}"
                    MethodName="get_InstrumentsComboSource"/>
```

---

## 🛠️ 開発環境

| 要件 | バージョン |
|------|-------------|
| .NET SDK | 8.0 以上 |
| OS | Windows 7 以降 |
| MahApps.Metro | 2.4.11 以上 |
| CommunityToolkit.Mvvm | 8.4.0 以上 |

---

## 📦 ビルドとパッケージ作成

```bash
dotnet build -c Release
```

ビルド時に自動的に `.nupkg` ファイル（NuGetパッケージ）が生成されます。  
これは `.csproj` の設定：

```xml
<GeneratePackageOnBuild>True</GeneratePackageOnBuild>
```

により自動化されています。

---

## 📄 ライセンス

このライブラリは [MIT License](LICENSE) で提供されています。

---

## 👤 作者

**Min Max**  
📦 [GitHub リポジトリ](https://github.com/MinMax25/libQB)

---

> 💡 このライブラリは、WPFアプリケーション開発を効率化するためのベースユーティリティです。  
> MahApps.MetroによるUI拡張と、MVVM Toolkitによるロジック整理を統合的に扱う開発基盤として利用できます。
```
