# Backup SQLite database on Azure App Service (Windows)
# Location after publish: D:\home\site\wwwroot\tools\backup.ps1
# Behavior: take site offline, copy DB + WAL/SHM, zip files, print download URLs via Kudu VFS

param()

$ErrorActionPreference = 'Stop'

function Write-Info($msg) { Write-Host "[INFO] $msg" }
function Write-Warn($msg) { Write-Host "[WARN] $msg" -ForegroundColor Yellow }
function Write-Err($msg)  { Write-Host "[ERROR] $msg" -ForegroundColor Red }

try {
    $siteRoot    = "D:\home\site\wwwroot"
    $appOffline  = Join-Path $siteRoot "app_offline.htm"
    $backupDir   = "D:\home\backups"
    if (-not (Test-Path $backupDir)) { New-Item -ItemType Directory -Path $backupDir -Force | Out-Null }

    $ts          = Get-Date -Format yyyyMMddHHmmss
    $dbPath      = $env:DB_PATH
    if ([string]::IsNullOrWhiteSpace($dbPath)) { $dbPath = "D:\home\data\drinkshop.db" }

    if (-not (Test-Path $dbPath)) {
        Write-Err "Database file not found: $dbPath"
        exit 1
    }

    Write-Info "Using DB: $dbPath"
    $createdOffline = $false

    # Take the site offline to release SQLite locks and ensure a consistent copy
    if (-not (Test-Path $appOffline)) {
        "offline" | Out-File -FilePath $appOffline -Encoding ascii -Force
        $createdOffline = $true
        Write-Info "Site taken offline via app_offline.htm"
        Start-Sleep -Seconds 3
    } else {
        Write-Warn "app_offline.htm already exists; site should already be offline"
    }

    $destBase = Join-Path $backupDir ("drinkshop_{0}" -f $ts)

    # Copy main DB
    $destDb = "$destBase.db"
    Copy-Item -Path $dbPath -Destination $destDb -Force

    # Copy WAL/SHM if present
    foreach ($ext in @("db-wal","db-shm")) {
        $src = [System.IO.Path]::ChangeExtension($dbPath, $ext)  # e.g., drinkshop.db-wal
        if (Test-Path $src) {
            $dest = "$destBase.$ext"
            Copy-Item -Path $src -Destination $dest -Force
        }
    }

    # Zip all copied artifacts for easier download
    $zipPath = "$destBase.zip"
    $toZip = Get-ChildItem "$destBase.*" -ErrorAction SilentlyContinue
    if ($toZip) {
        Compress-Archive -Path $toZip.FullName -DestinationPath $zipPath -Force
        Write-Info "Created zip: $zipPath"
    } else {
        Write-Warn "No copied artifacts found to zip"
    }

    # Print local paths
    Write-Host ""; Write-Info "Backup files:"
    Get-ChildItem "$destBase.*" | ForEach-Object { Write-Host $_.FullName }

    # Print Kudu VFS download URLs (if WEBSITE_SITE_NAME is available)
    $siteName = $env:WEBSITE_SITE_NAME
    if (-not [string]::IsNullOrWhiteSpace($siteName)) {
        $baseUrl = "https://$siteName.scm.azurewebsites.net/api/vfs/backups"
        Write-Host ""; Write-Info "Download URLs (Kudu VFS):"
        Get-ChildItem "$destBase.*" | ForEach-Object {
            $name = Split-Path $_.FullName -Leaf
            Write-Host "$baseUrl/$name"
        }
    } else {
        Write-Warn "WEBSITE_SITE_NAME not set; open Kudu and browse to D:\home\backups to download."
    }

    Write-Host ""; Write-Info "Backup completed."
}
catch {
    Write-Err $_
    exit 1
}
finally {
    # Bring site back online only if we created app_offline.htm
    try {
        if ($createdOffline -and (Test-Path $appOffline)) {
            Remove-Item $appOffline -Force
            Write-Info "Site brought back online (app_offline.htm removed)"
        }
    } catch { Write-Warn "Failed to remove app_offline.htm: $_" }
}
