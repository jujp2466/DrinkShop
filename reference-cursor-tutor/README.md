# 清涼飲品銷售網站 - Vue 3 + Vite 版本

一個現代化的飲料銷售網站，使用 Vue 3 + Vite 前端框架和 Node.js + Express 後端 API。

## 🚀 技術棧

### 前端 (Vue 3 + Vite)
- **Vue 3** - 現代化的前端框架
- **Vite** - 快速的構建工具
- **Vue Router 4** - 官方路由管理器
- **Pinia** - 狀態管理庫
- **Axios** - HTTP 客戶端
- **CSS3** - 現代化樣式

### 後端 (Node.js + Express)
- **Node.js** - JavaScript 運行時
- **Express.js** - Web 應用框架
- **SQLite3** - 輕量級數據庫
- **JWT** - 身份驗證
- **Multer** - 文件上傳
- **bcryptjs** - 密碼加密

## ✨ 功能特色

### 🛒 前端購物功能
- **響應式設計** - 支援桌面和移動設備
- **產品展示** - 美觀的產品卡片佈局
- **用戶認證** - 登入/註冊系統
- **購物車** - 完整的購物車功能
- **模態框** - 現代化的用戶界面

### 🔧 後台管理系統
- **儀表板** - 數據統計和概覽
- **產品管理** - 新增、編輯、刪除產品
- **訂單管理** - 查看和更新訂單狀態
- **用戶管理** - 查看註冊用戶
- **文件上傳** - 產品圖片上傳

### 🔐 安全功能
- **JWT 認證** - 安全的身份驗證
- **密碼加密** - bcrypt 密碼哈希
- **權限控制** - 管理員和用戶權限分離
- **API 保護** - 受保護的後端 API

## 📦 安裝和運行

### 1. 克隆項目
```bash
git clone <repository-url>
cd beverage-store
```

### 2. 安裝依賴
```bash
npm install
```

### 3. 啟動開發服務器

#### 方法一：使用啟動腳本（推薦）
```bash
start-vue.bat
```

#### 方法二：手動啟動
```bash
# 終端 1：啟動後端服務器
npm start

# 終端 2：啟動前端開發服務器
npm run dev:frontend
```

### 4. 訪問網站
- **前端網站**: http://localhost:5173
- **後台管理**: http://localhost:5173/admin
- **API 服務器**: http://localhost:3000

## 🔑 默認帳戶

### 管理員帳戶
- **用戶名**: `admin`
- **密碼**: `admin123`

## 📁 項目結構

```
beverage-store/
├── src/                    # Vue 3 源代碼
│   ├── components/         # Vue 組件
│   │   ├── LoginModal.vue
│   │   ├── RegisterModal.vue
│   │   ├── CartModal.vue
│   │   └── ProductModal.vue
│   ├── views/             # 頁面組件
│   │   ├── Home.vue       # 首頁
│   │   ├── Admin.vue      # 後台管理
│   │   ├── Login.vue      # 登入頁面
│   │   └── Register.vue   # 註冊頁面
│   ├── stores/            # Pinia 狀態管理
│   │   ├── auth.js        # 認證狀態
│   │   ├── cart.js        # 購物車狀態
│   │   └── products.js    # 產品狀態
│   ├── utils/             # 工具函數
│   │   └── api.js         # API 配置
│   ├── assets/            # 靜態資源
│   │   └── main.css       # 主樣式
│   ├── router/            # 路由配置
│   │   └── index.js
│   ├── App.vue            # 根組件
│   └── main.js            # 應用入口
├── server.js              # Express 後端服務器
├── vite.config.js         # Vite 配置
├── package.json           # 項目配置
├── index.html             # HTML 模板
├── uploads/               # 上傳文件目錄
└── beverage_store.db      # SQLite 數據庫
```

## 🔧 開發命令

```bash
# 開發模式
npm run dev:frontend    # 啟動前端開發服務器
npm run dev             # 啟動後端開發服務器

# 構建
npm run build           # 構建生產版本
npm run preview         # 預覽構建結果

# 生產模式
npm start               # 啟動生產服務器
```

## 🌐 API 端點

### 認證相關
- `POST /api/auth/login` - 用戶登入
- `POST /api/auth/register` - 用戶註冊
- `GET /api/auth/me` - 獲取當前用戶信息

### 產品相關
- `GET /api/products` - 獲取所有產品
- `GET /api/products/:id` - 獲取單個產品
- `POST /api/products` - 新增產品（管理員）
- `PUT /api/products/:id` - 更新產品（管理員）
- `DELETE /api/products/:id` - 刪除產品（管理員）

### 訂單相關
- `POST /api/orders` - 創建訂單
- `GET /api/orders` - 獲取訂單列表
- `PUT /api/admin/orders/:id/status` - 更新訂單狀態（管理員）

## 🎨 設計特色

### 視覺設計
- **現代化 UI** - 清爽的藍色主題
- **響應式佈局** - 適配各種屏幕尺寸
- **動畫效果** - 平滑的過渡動畫
- **用戶友好** - 直觀的操作界面

### 用戶體驗
- **快速加載** - Vite 的快速熱重載
- **無縫導航** - Vue Router 的 SPA 體驗
- **狀態管理** - Pinia 的響應式狀態
- **錯誤處理** - 完善的錯誤提示

## 🚀 部署

### 開發環境
1. 確保已安裝 Node.js (v16+)
2. 運行 `npm install` 安裝依賴
3. 使用 `start-vue.bat` 或手動啟動服務器

### 生產環境
1. 運行 `npm run build` 構建前端
2. 將 `dist` 目錄內容部署到 Web 服務器
3. 配置後端服務器運行 `npm start`

## 🤝 貢獻

歡迎提交 Issue 和 Pull Request！

## 📄 許可證

MIT License

---

**享受您的清涼飲品！** 🥤

