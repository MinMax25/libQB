# libQB

WPF アプリケーション「QB」向けに開発された .NET ライブラリです。
CommunityToolkit.Mvvm と MahApps.Metro をベースに、MVVM 補助、ダイアログ制御、Undo/Redo 機構などを提供します。

主な機能：
- MVVM支援
- ダイアログ／ウィンドウサービス
- Undo / Redo 機構
- データバインディング支援
- MahApps.Metro 拡張

プロジェクト構成例：
- libQB/
  - libQB.UndoRedo/
  - libQB.IO/
  - libQB.Common/
  - libQB.Audio/
  - Properties/
  - libQB.csproj

インストール：
1. NuGet パッケージとしてインストール：
   dotnet add package libQB
2. または Visual Studio の [NuGet パッケージの管理] から「libQB」を検索してインストール

使用例：
// ダイアログ表示
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

// Undo / Redo
var history = new UndoHistory();
history.Execute(new PropertyChangeAction<MyViewModel>(
    target, nameof(target.Property), oldValue, newValue));

開発環境：
- .NET SDK 8.0 以上
- Windows 7 以降
- MahApps.Metro 2.4.11 以上
- CommunityToolkit.Mvvm 8.4.0 以上

ビルド：
dotnet build -c Release

ライセンス：MIT

作者：Min Max
GitHub：https://github.com/MinMax25/libQB
