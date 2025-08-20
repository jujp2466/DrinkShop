$OutputEncoding = [System.Text.Encoding]::UTF8 # 確保 PowerShell 的輸出編碼為 UTF-8

# 更新 SQLite 資料庫腳本
# 1. 執行 EF Core Migrations，確保資料庫結構與最新的 Entity 同步
# 2. 備份現有資料庫（可選）
# 3. 如果資料庫結構不符，刪除並重新建立資料庫

# 設定專案路徑與資料庫檔案路徑
$projectPath = "D:\Lab\DrinkShop"
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

# 刪除現有資料庫
if (Test-Path -Path $dbFilePath) {
    Remove-Item -Path $dbFilePath -Force
    Write-Host "已刪除現有資料庫: $dbFilePath"
}

# 重新建立資料庫
Write-Host "開始重新建立資料庫..."
Set-Location -Path $projectPath
try {
    # 自動新增遷移以捕捉模型變更
    $migrationName = "AutoMigration_$(Get-Date -Format 'yyyyMMddHHmmss')"
    Write-Host "正在新增遷移: $migrationName"
    dotnet ef migrations add $migrationName --project "DrinkShop.Infrastructure" --startup-project "DrinkShop.Api"

    # 套用遷移以更新/建立資料庫
    Write-Host "正在更新資料庫..."
    dotnet ef database update --project "DrinkShop.Infrastructure" --startup-project "DrinkShop.Api"
    Write-Host "資料庫重新建立完成！"
} catch {
    Write-Host "資料庫重新建立失敗: $_"
}
