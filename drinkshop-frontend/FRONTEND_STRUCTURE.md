# DrinkShop 前端 (drinkshop-frontend) 專案結構說明

簡短目的
- 單頁應用 (Vue 3 + Vite)，負責產品頁面、購物車、結帳，並與後端 API 通訊。

快速上手
- 安裝與開發
  - npm install
  - npm run dev
- 打包（production）
  - npm run build
  - 結果會產生 `dist/`，可部署為靜態站點（或上傳至專用靜態主機）。

專案目錄快覽 (tree)
```txt
drinkshop-frontend/
├─ index.html
├─ package.json
├─ vite.config.js
├─ src/
│  ├─ main.js
│  ├─ App.vue
│  ├─ api.js
│  ├─ style.css
│  ├─ assets/
│  └─ components/
│     ├─ OrderPage.vue
│     └─ DrinkCrud.vue
└─ dist/ (build 輸出)
```

主要檔案與目錄
- `index.html`：Vite 的入口 HTML。
- `package.json`：npm 腳本、相依套件與專案 metadata。
- `vite.config.js`：Vite 設定。
- `src/`
  - `main.js`：Vue app 啟動，掛載 router 與全域設定。
  - `App.vue`：根元件（主要使用 `<router-view />`）。
  - `api.js`：與後端 API 的封裝（base URL 設定點）。
  - `style.css`：全域樣式（共享 header/footer 與元件樣式）。
  - `components/`
    - `OrderPage.vue`：主要消費者介面（產品列表、購物車、結帳）。
    - `DrinkCrud.vue`：管理介面（新增/刪除飲品）。
  - `router/`
    - `index.js`：SPA 路由設定（例如 `/` -> `OrderPage`、`/drinkcrud` -> `DrinkCrud`）。
  - `assets/`：靜態資源（SVG、圖示等）。

重要行為/注意事項
- Routing：內部導覽應使用 `<router-link>` 或 programmatic `router.push`，避免使用傳統 `<a href="/...">` 導致整頁重新載入。
- 欄位命名兼容性：後端屬性 `PurchaseCount` 可能大小寫不同，前端使用 `(drink.PurchaseCount ?? drink.purchaseCount) ?? 0` 的容錯處理。
- Node 版本：生產 build 可能依賴特定 Node 版本（專案測試使用 Node v20.x）。若遇到 `crypto.hash is not a function` 等錯誤，請檢查 Node 版本。
- 部署：若把前端佈署在與 API 不同的域名，請在後端啟用對應的 CORS 並設定 API_BASE_URL。

常見命令摘要
- 安裝套件：`npm install`
- 本機開發：`npm run dev`（Vite dev server）
- 生產打包：`npm run build`（產出 `dist/`）
- 清除並重新安裝 (PowerShell)：
```
Remove-Item -Recurse node_modules
Remove-Item package-lock.json
npm install
```

CI / GitHub Actions 範例（已在 repo 新增 workflow）
- 檔案：`.github/workflows/frontend-ci.yml`
- 功能：在 push 或 PR 時安裝、build，並上傳 `dist/` 作為 artifact；也可加入部署步驟（需設定 Azure/StaticWebApp secrets）。

部署建議
- Azure Static Web Apps：推薦用於單頁靜態網站（可與後端分離）。需設定 app_location=`/`, api_location 空 (後端獨立部署)，並提供 `azure_static_web_apps_api_token` 作為 secret。
- Azure Blob / CDN / GitHub Pages：將 `dist/` 上傳到靜態站點即可。若需要 https、CDN，建議透過 Azure Storage + CDN 或 Netlify / Vercel 等專門服務。

可擴充/建議
- 建議在 `README.md` 加上 API_URL 範例與 env 範本，方便 CI/CD 配置。


