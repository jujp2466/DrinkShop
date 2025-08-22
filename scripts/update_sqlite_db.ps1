<#
<#
安全版：更新 SQLite 資料庫腳本

功能：
 - 預設使用環境變數 `DB_PATH`（若無則回退到專案內的本地路徑）。
 - 會先備份現有資料庫（若存在）。
 - 預設只套用 migration（dotnet ef database update），不會自動新增 migration，也不會刪除 DB 檔。
 - 只有在明確指定 `-ForceDelete` 時才會刪除 DB 檔；若在 Azure App Service (有 HOME 環境變數) 上執行，預設會拒絕刪除並要求明確確認。
 - 提供 `-AddMigration` 選項以在安全情況下新增 migration（建議只在本機或 staging 使用）。

用法範例：
 .\update_sqlite_db.ps1                      # 只備份並套用 migration
 .\update_sqlite_db.ps1 -AddMigration      # 在本機新增 migration 並套用（仍不會刪除 DB）
 .\update_sqlite_db.ps1 -ForceDelete       # 會刪除 DB（需謹慎；在 Azure 上會額外提示）
#>

[CmdletBinding()]
param(
    [switch]$ForceDelete,
    [switch]$AddMigration,
    [string]$ProjectPath = "D:\Lab\DrinkShop",
    [string]$BackupDir = "$env:TEMP\drinkshop_backups"
)

# 取得 DB 路徑：優先使用環境變數 DB_PATH
$dbFilePath = $env:DB_PATH
if ([string]::IsNullOrEmpty($dbFilePath)) {
    $dbFilePath = Join-Path -Path $ProjectPath -ChildPath "DrinkShop.Api\data\drinkshop.db"
    Write-Host "未設定 DB_PATH，使用預設本機路徑： $dbFilePath"
} else {
    Write-Host "使用環境變數 DB_PATH： $dbFilePath"
}

# 偵測是否在 Azure App Service（簡單依 HOME env 判斷）
$isAzure = -not [string]::IsNullOrEmpty($env:HOME)
if ($isAzure) { Write-Host "偵測到 Azure App Service 環境 (HOME=$env:HOME)。會更嚴格保護資料庫。" }

# 確保備份資料夾存在
if (!(Test-Path -Path $BackupDir)) {
    try {
        New-Item -ItemType Directory -Path $BackupDir -Force | Out-Null
    } catch {
        Write-Host "無法建立備份資料夾 $BackupDir： $_"; exit 1
    }
}

# 備份現有資料庫
if (Test-Path -Path $dbFilePath) {
    try {
        $timestamp = Get-Date -Format "yyyyMMddHHmmss"
        $backupFile = Join-Path -Path $BackupDir -ChildPath "drinkshop_$timestamp.db"
        Copy-Item -Path $dbFilePath -Destination $backupFile -Force -ErrorAction Stop
        Write-Host "資料庫已備份至: $backupFile"
    } catch {
        Write-Host "備份失敗: $_"
        if (-not $ForceDelete) {
            Write-Host "中止：未能建立備份，請檢查權限或路徑。"; exit 1
        } else {
            Write-Host "--ForceDelete 已指定，繼續進行刪除作業。"
        }
    }
} else {
    Write-Host "未找到資料庫檔案，將建立新的資料庫： $dbFilePath"
}

# 刪除動作需要明確指定 --ForceDelete
if ($ForceDelete) {
    if ($isAzure) {
        Write-Host "注意：您在 Azure 環境，ForceDelete 會刪除在 Azure 上的資料庫檔案。除非你確定已備份，否則不要在 production 上執行此選項。"
        $confirm = Read-Host "請輸入 YES 以確認刪除並繼續 (其他輸入將中止)"
        if ($confirm -ne 'YES') { Write-Host "已中止（使用者未確認）。"; exit 1 }
    } else {
        $confirm = Read-Host "確定要刪除 local DB 檔案 $dbFilePath ? 輸入 YES 繼續"
        if ($confirm -ne 'YES') { Write-Host "已中止（使用者未確認）。"; exit 1 }
    }

    if (Test-Path -Path $dbFilePath) {
        try {
            Remove-Item -Path $dbFilePath -Force -ErrorAction Stop
            Write-Host "已刪除資料庫： $dbFilePath"
        } catch {
            Write-Host "刪除失敗： $_"; exit 1
        }
    } else {
        Write-Host "要刪除的檔案不存在： $dbFilePath"
    }
} else {
    Write-Host "未指定 -ForceDelete，將不刪除資料庫檔案。"
}

# 若要新增 migration，僅在非 Azure 環境或由開發者在本機執行時使用
Set-Location -Path $ProjectPath
if ($AddMigration) {
    if ($isAzure) {
        Write-Host "警告：在 Azure 上自動新增 migration 不建議執行。中止新增 migration。"
    } else {
        $migrationName = "AutoMigration_$(Get-Date -Format 'yyyyMMddHHmmss')"
        Write-Host "正在新增遷移: $migrationName"
        try {
            dotnet ef migrations add $migrationName --project "DrinkShop.Infrastructure" --startup-project "DrinkShop.Api"
        } catch {
            Write-Host "新增遷移失敗： $_"; Write-Host "請在本機修正後再執行。"; exit 1
        }
    }
}

# 套用 migration（更新資料庫）
Write-Host "開始套用 migration（dotnet ef database update）..."
try {
    dotnet ef database update --project "DrinkShop.Infrastructure" --startup-project "DrinkShop.Api"
    Write-Host "資料庫套用完成。"
} catch {
    Write-Host "套用 migration 失敗： $_"; exit 1
}

Write-Host "完成。備份位於： $BackupDir。若需要下載備份或還原，請立即取出備份檔。"
