# Desktop Task Widget

Windowsデスクトップ右下に控えめに常駐する、業務配布向けの軽量ToDoウィジェットです。

## 方針

- C# / WPF / .NET Framework 4.8
- SQLiteによるローカル保存
- Windows同梱の `winsqlite3` を利用し、同梱DLLの肥大化を抑制
- ユーザー単位の設定とデータ保存
- 管理者権限なしで使えるスタートアップ登録
- タスクトレイ常駐

## データ保存先

- DB: `%LocalAppData%\DesktopTaskWidget\tasks.db`
- 設定: `%LocalAppData%\DesktopTaskWidget\settings.json`

## スタートアップ

設定チェックにより、以下のユーザー単位レジストリへ登録します。

`HKCU\Software\Microsoft\Windows\CurrentVersion\Run`

登録値には `--startup` 引数を付与し、通常起動と自動起動を区別します。

## ビルド

Visual Studio 2022以降で `DesktopTaskWidget/DesktopTaskWidget.csproj` を開いてビルドしてください。

このプロジェクトは `.NET Framework 4.8` を対象にしているため、ビルド環境には .NET Framework 4.8 Developer Pack が必要です。

コマンドラインでは以下でも確認できます。

```powershell
dotnet restore .\DesktopTaskWidget.sln
dotnet build .\DesktopTaskWidget.sln
```
