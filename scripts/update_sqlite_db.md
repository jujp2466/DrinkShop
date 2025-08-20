# 更新 SQLite 資料庫代理程式

此代理程式用於在修改 Entity 後，自動執行必要的更新步驟以同步資料庫結構。

## 使用方式
1. 確保已安裝 .NET SDK。
2. 確保 `DrinkShop.Api` 專案的 `Program.cs` 已正確配置資料庫連線。
3. 在終端執行以下命令：

```powershell
cd D:\Lab\DrinkShop
powershell -File .\scripts\update_sqlite_db.ps1
```

## 代理程式內容
以下是 PowerShell 腳本內容，請將其保存為 `update_sqlite_db.ps1`。

```powershell
# 更新 SQLite 資料庫腳本
# 1. 執行 EF Core Migrations，確保資料庫結構與最新的 Entity 同步
# 2. 備份現有資料庫（可選）

# 設定專案路徑與資料庫檔案路徑
$projectPath = "D:\Lab\DrinkShop\DrinkShop.Api"
$backupPath = "D:\Lab\DrinkShop\DrinkShop.Api\data\backup"
$dbFilePath = "D:\Lab\DrinkShop\DrinkShop.Api\data\drinkshop.db"

# 確保備份資料夾存在
if (!(Test-Path -Path $backupPath)) {
    New-Item -ItemType Directory -Path $backupPath | Out-Null
}

# 備份現有資料庫
if (Test-Path -Path $dbFilePath) {
    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    $backupFile = "$backupPath\drinkshop_$timestamp.db"
    Copy-Item -Path $dbFilePath -Destination $backupFile
    Write-Host "資料庫已備份至: $backupFile"
}

# 執行 EF Core Migrations 更新資料庫
Write-Host "開始更新資料庫..."
cd $projectPath
try {
    dotnet ef database update --project "DrinkShop.Infrastructure" --startup-project "DrinkShop.Api"
    Write-Host "資料庫更新完成！"
} catch {
    Write-Host "資料庫更新失敗: $_"
}
```

## 注意事項
- 請確保 `dotnet ef` 工具已安裝，否則執行以下命令安裝：

```powershell
dotnet tool install --global dotnet-ef
```

- 如果資料庫結構有重大變更，請先新增 Migration：

```powershell
cd D:\Lab\DrinkShop
cd DrinkShop.Infrastructure
dotnet ef migrations add <MigrationName> --project "DrinkShop.Infrastructure" --startup-project "DrinkShop.Api"
```
