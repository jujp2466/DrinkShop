# 清涼飲品 - 現代化飲料銷售網站

一個完整的飲料銷售網站，包含前端展示和後台管理系統。

## 功能特色

### 前端功能
- 🛍️ **購物車系統** - 完整的購物車功能，支持數量調整和商品管理
- 👤 **用戶認證** - 用戶註冊、登入、登出功能
- 📱 **響應式設計** - 完美適配桌面和移動設備
- 🎨 **現代化UI** - 美觀的界面設計，流暢的動畫效果
- 💳 **訂單系統** - 完整的下單流程

### 後台管理系統
- 📊 **儀表板** - 實時統計數據展示
- 📦 **產品管理** - 新增、編輯、刪除產品，支持圖片上傳
- 📋 **訂單管理** - 查看訂單詳情，更新訂單狀態
- 👥 **用戶管理** - 管理用戶帳戶和權限
- 🔍 **篩選搜尋** - 強大的篩選和搜尋功能

## 技術架構

- **後端**: Node.js + Express.js
- **數據庫**: SQLite3
- **前端**: 原生JavaScript + HTML5 + CSS3
- **認證**: JWT (JSON Web Token)
- **文件上傳**: Multer
- **密碼加密**: bcryptjs

## 安裝說明

### 前置需求
- Node.js (版本 14 或以上)
- npm 或 yarn

### 安裝步驟

1. **克隆項目**
```bash
git clone <repository-url>
cd beverage-store
```

2. **安裝依賴**
```bash
npm install
```

3. **啟動服務器**
```bash
npm start
```

4. **開發模式**
```bash
npm run dev
```

## 使用說明

### 訪問網站
- 前端網站: http://localhost:3000
- 後台管理: http://localhost:3000/admin

### 默認管理員帳戶
- 用戶名: `admin`
- 密碼: `admin123`

### 功能演示

#### 前端購物流程
1. 瀏覽產品目錄
2. 將商品加入購物車
3. 調整商品數量
4. 登入或註冊帳戶
5. 完成結帳

#### 後台管理
1. 使用管理員帳戶登入後台
2. 在儀表板查看統計數據
3. 管理產品庫存和價格
4. 處理客戶訂單
5. 管理用戶帳戶

## 項目結構

```
beverage-store/
├── server.js              # 主服務器文件
├── package.json           # 項目配置
├── README.md             # 項目說明
├── public/               # 前端文件
│   ├── index.html        # 主頁面
│   ├── admin.html        # 後台管理頁面
│   ├── styles.css        # 前端樣式
│   ├── admin-styles.css  # 後台樣式
│   ├── app.js           # 前端JavaScript
│   └── admin.js         # 後台JavaScript
├── uploads/              # 上傳文件目錄
└── beverage_store.db     # SQLite數據庫文件
```

## API 端點

### 用戶認證
- `POST /api/register` - 用戶註冊
- `POST /api/login` - 用戶登入

### 產品管理
- `GET /api/products` - 獲取所有產品
- `GET /api/products/:id` - 獲取單個產品
- `POST /api/admin/products` - 新增產品 (管理員)
- `PUT /api/admin/products/:id` - 更新產品 (管理員)
- `DELETE /api/admin/products/:id` - 刪除產品 (管理員)

### 訂單管理
- `POST /api/orders` - 創建訂單
- `GET /api/orders` - 獲取訂單列表
- `PUT /api/admin/orders/:id/status` - 更新訂單狀態 (管理員)

## 數據庫結構

### 用戶表 (users)
- id, username, password, email, role, created_at

### 產品表 (products)
- id, name, description, price, category, image_url, stock, is_active, created_at

### 訂單表 (orders)
- id, user_id, total_amount, status, created_at

### 訂單詳情表 (order_items)
- id, order_id, product_id, quantity, price

## 自定義配置

### 環境變量
創建 `.env` 文件來配置環境變量：

```env
PORT=3000
JWT_SECRET=your-secret-key
```

### 數據庫配置
項目使用 SQLite 數據庫，數據庫文件會在首次運行時自動創建。

## 部署說明

### 本地部署
1. 確保已安裝 Node.js
2. 運行 `npm install` 安裝依賴
3. 運行 `npm start` 啟動服務器

### 生產環境部署
1. 設置環境變量
2. 使用 PM2 或類似工具管理進程
3. 配置反向代理 (如 Nginx)
4. 設置 SSL 證書

## 開發指南

### 添加新功能
1. 在 `server.js` 中添加新的 API 端點
2. 更新前端 JavaScript 文件
3. 修改相應的 HTML 和 CSS 文件

### 數據庫遷移
項目使用 SQLite，數據庫結構在 `server.js` 中定義。如需修改，請更新相應的 CREATE TABLE 語句。

## 故障排除

### 常見問題

1. **端口被佔用**
   - 修改 `.env` 文件中的 PORT 變量
   - 或終止佔用端口的進程

2. **數據庫錯誤**
   - 刪除 `beverage_store.db` 文件重新創建
   - 檢查數據庫權限

3. **文件上傳失敗**
   - 確保 `uploads` 目錄存在且有寫入權限
   - 檢查文件大小限制

## 貢獻指南

1. Fork 項目
2. 創建功能分支
3. 提交更改
4. 發起 Pull Request

## 授權

MIT License

## 聯繫方式

如有問題或建議，請聯繫開發團隊。

---

**享受您的飲料購物體驗！** 🥤
