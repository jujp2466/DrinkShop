# DrinkShop 後端 (DrinkShop.Api) 專案結構說明

簡短目的
- ASP.NET Core Web API (net9) + EF Core (SQLite)：負責資料存取、商業邏輯與 API 提供。

快速上手
- 開發/編譯
  - dotnet build
  - dotnet run --project DrinkShop.Api
- 資料庫遷移（使用 EF Core migrations）
  - 新增 migration：
```
dotnet ef migrations add <Name> --project DrinkShop.Infrastructure --startup-project DrinkShop.Api
```
  - 套用 migration：
```
dotnet ef database update --project DrinkShop.Infrastructure --startup-project DrinkShop.Api
```

專案目錄快覽 (tree)
```txt
DrinkShop/
├─ DrinkShop.Api/
│  ├─ Program.cs
│  ├─ Controllers/
│  │  └─ DrinkController.cs
│  ├─ wwwroot/
│  │  └─ tools/backup.ps1
│  └─ DrinkShop.Api.csproj
├─ DrinkShop.Infrastructure/
│  ├─ DrinkShopDbContext.cs
│  └─ Migrations/
├─ DrinkShop.Domain/
│  └─ Entities/Drink.cs
└─ DrinkShop.Application/
   └─ Services/
```

主要檔案與目錄
- `DrinkShop.Api/`
  - `Program.cs`：應用啟動，設定 DI、DbContext 與讀取環境變數（ex: `DB_PATH`）。程式啟動會呼叫 `db.Database.Migrate()` 以套用尚未套用的遷移。
  - `appsettings.json` / `appsettings.Development.json`：設定檔。
  - `Controllers/DrinkController.cs`：提供飲品 CRUD 與 `checkout` 等 API。
  - `wwwroot/`：靜態檔與運維腳本（`tools/backup.ps1`）。
  - `DrinkShop.Api.csproj`：專案檔。
- `DrinkShop.Infrastructure/`
  - `DrinkShopDbContext.cs`：EF Core DbContext 與 DbSet 定義。
  - `Migrations/`：EF migration 檔案（請將 migration 版控，方便團隊追蹤 schema 變更）。
- `DrinkShop.Domain/`
  - `Entities/Drink.cs`：領域實體（現包含 `PurchaseCount`、`Stock`、`Price` 等欄位）。
- `DrinkShop.Application/`
  - `Services/DrinkService.cs`、`Interfaces/`：應用層接口與服務實作。

重要行為/注意事項
- SQLite 持久化（Azure App Service）
  - 在 Azure 上建議把資料庫放在可持久化的路徑（示例）：`D:\home\data\drinkshop.db`。
  - 專案會從環境變數 `DB_PATH` 讀取資料庫路徑，若未設定會 fallback 到系統預設（程式碼中會嘗試使用 `HOME` 或 `D:\home`）。
- Migrations：請將 migration 檔案加入版本控制。程式啟動時執行 `Database.Migrate()` 可自動套用非破壞性遷移；具有破壞性的變更（drop/rename）需人工 review 並提前備份。
- Git：不要將 SQLite DB 檔（例如 `*.db`、`*.db-shm`、`*.db-wal`）提交至版本控制，檔案已加入 `.gitignore`。
- 運維腳本
  - `scripts/update_sqlite_db.ps1`：在本機或 CI 上以安全方式備份並套用遷移的 PowerShell 腳本。
  - `wwwroot/tools/backup.ps1`：可在 App Service 的 Kudu/console 執行以離線備份資料庫（腳本會建立 `app_offline.htm`、複製 db + wal/shm、zip 並輸出下載連結）。

部署 / CI 建議與範例
- Azure App Service（後端）部署建議
  1. 在 App Service 的 Application Settings 中加入 `DB_PATH` = `D:\home\data\drinkshop.db`。
  2. 先在生產環境執行 `backup.ps1` 或手動備份資料庫檔案。
  3. 使用 `dotnet publish -c Release -o publish` 建置，然後用 Web Deploy 或 Publish Profile 將 `publish/` 部署到 App Service。
  4. 在 GitHub Actions 中可使用 `azure/webapps-deploy@v2` 並把 publish profile 設為 secret（`AZURE_WEBAPP_PUBLISH_PROFILE`）。

- GitHub Actions 範例（已在 repo 新增 workflow）
  - 檔案：`.github/workflows/backend-ci.yml`
  - 功能：在 push/PR 時還原、建置、並產出發佈 artifact；若配置 publish-profile 可直接部署到 App Service。

環境與敏感資訊
- 在 GitHub Actions 部署到 Azure 時，請在 repository Secrets 加入：
  - `AZURE_WEBAPP_PUBLISH_PROFILE`（若使用 Publish Profile 部署）
  - `AZURE_STATIC_WEB_APPS_API_TOKEN`（若使用 Static Web Apps 部署前端）

快速檢查清單（發生問題時）
- 檢查 App Service 的 App Settings 是否有 `DB_PATH` 並指向 `D:\home\data\drinkshop.db`。
- 使用 Kudu Console（/scm）登入並檢查 `D:\home\data` 與 `D:\home\site\wwwroot`。
- 若遷移失敗，先檢查 log（App Service logs / stdout log），並還原備份再調查 migration SQL。

補充：設計決策與最佳實務
- Migrations 應該進入版本庫以保留 schema 歷史。
- 針對 production DB 操作，永遠先備份並在非高峰時段進行。
- 若未來需要可擴充到 Azure SQL 或其他 RDBMS，Abstract DbContext/Repository 層以便替換提供便利。


