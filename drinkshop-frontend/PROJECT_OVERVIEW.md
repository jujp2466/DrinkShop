# 前端專案架構與技術解析 (PROJECT_OVERVIEW.md)

這份文件旨在協助您在面試時，清晰地介紹此 Vue 3 前端專案的架構、技術選型與核心邏輯。

---

## 1. 專案概覽

本專案是一個飲料店的線上點餐系統前端，採用 **Vue 3** 的 **Composition API** 進行開發，搭配現代化的前端工具鏈，旨在打造一個高效、可維護、且具備良好使用者體驗的單頁應用程式 (SPA)。

- **主要技術棧**:
  - **框架**: Vue 3 (使用 `<script setup>` 的 Composition API)
    
*   **依賴注入 (DI)**: 在後端使用 ASP.NET Core 的 DI 容器（例如 `builder.Services.AddScoped`）註冊服務，可在面試中說明如何以 interface 注入不同實作；前端則以 Pinia 與組件抽象化來達成類似的模組化設計。

  - **建置工具**: Vite (提供極速的開發伺服器和打包體驗)
  - **狀態管理**: Pinia (Vue 官方推薦的下一代狀態管理庫)
  - **路由**: Vue Router (官方 SPA 路由管理器)
  - **API 請求**: Axios (封裝在 `src/api.js` 中，方便統一管理)
  - **樣式**: 純 CSS，搭配 Font Awesome 圖示庫

---

## 2. 專案結構 (src)

清晰的目錄結構是專案可維護性的基礎。

- `main.js`: **程式進入點**。初始化 Vue 應用，並掛載 Pinia 和 Vue Router。
- `App.vue`: **根組件**。包含 `<router-view>`，所有頁面都會在這裡被渲染。
  - `api.js`: **API 請求中心**。集中管理 Axios 實例，統一設定 baseURL 和攔截器 (interceptor)。本專案支援 JWT 驗證，登入後所有 API 請求皆自動帶入授權憑證，確保安全性。

    後端以 RESTful API 風格提供服務，前端透過 `api.js` 呼叫語意化路由並使用標準 HTTP 方法 (GET/POST/PUT/DELETE) 與後端通訊。
- `router/index.js`: **路由設定檔**。定義所有頁面路徑與其對應的組件。
- `stores/`: **Pinia 狀態管理中心**。這是前端的核心數據層。
  - `auth.js`: 管理使用者登入狀態、JWT 和用戶資訊。登入後自動保存授權憑證，所有 API 請求皆自動驗證身分。
  - `cart.js`: 管理購物車的商品、數量和總金額。
  - `product.js`: 管理商品列表的數據和篩選狀態。
- `views/`: **頁面級組件**。每個檔案對應一個路由，例如 `HomePage.vue`, `ProductsPage.vue`。
- `components/`: **可重用 UI 組件**。例如 `ProductCard.vue` (商品卡片)、`CartSidebar.vue` (購物車側邊欄)、`AuthModals.vue` (登入/註冊彈窗)。
- `layouts/`: **佈局組件**。用來包裹 `views`，提供一致的頁面結構，例如 `MainLayout.vue` 包含共用的導覽列和頁尾。

---

## 3. 程式進入點與渲染流程

您可以透過一個頁面的渲染流程，來展示您對 Vue SPA 運作原理的理解。

**範例：使用者訪問「商品列表頁」 (`/products`)**

1.  **進入點 (`main.js`)**:
    - 應用程式啟動，建立 Vue 實例。
    - `app.use(router)` 和 `app.use(createPinia())` 將路由和狀態管理功能整合進應用。

2.  **Vue Router (`router/index.js`)**:
    - 路由器根據 URL `/products` 匹配到設定好的路由規則。
    - 找到對應的組件：`ProductsPage.vue`。

3.  **根組件 (`App.vue`)**:
    - 根組件內的 `<router-view>` 是一個佔位符，Vue Router 會將匹配到的 `ProductsPage.vue` 渲染到這個位置。

4.  **佈局與視圖 (`MainLayout.vue` & `ProductsPage.vue`)**:
    - `ProductsPage.vue` 可能被包裹在 `MainLayout.vue` 中，從而共用導覽列和頁尾。
    - `ProductsPage.vue` 在其 `<script setup>` 中：
      - 引入 `useProductStore()`。
      - 呼叫 `productStore.fetchProducts()` action 來觸發 API 請求。

5.  **狀態管理與 API (`stores/product.js` & `api.js`)**:
    - `fetchProducts()` action 透過 `api.js` 封裝的 Axios 實例向後端 `GET /api/products` 發送請求。
    - 請求成功後，將返回的商品數據存入 store 的 `state` 中。

6.  **響應式渲染**:
    - `ProductsPage.vue` 的模板中使用 `v-for` 遍歷從 store 中獲取的商品列表。
    - 由於 Pinia 的 state 是響應式的，當數據被存入 store 後，頁面會自動更新，將商品卡片 (`ProductCard.vue`) 渲染出來。

---

## 4. 核心功能邏輯：Pinia 狀態管理

Pinia 是本專案的亮點，您可以強調它如何簡化狀態管理。

**範例：使用者「加入購物車」**

1.  **觸發 (`ProductCard.vue`)**:
    - 使用者點擊「加入購物車」按鈕。
    - 按鈕的 `@click` 事件呼叫一個本地方法 `handleAddToCart`。

2.  **呼叫 Action**:
    - `handleAddToCart` 方法中，首先 `import { useCartStore } from '@/stores/cart'`。
    - 建立 store 實例：`const cartStore = useCartStore()`。
    - 呼叫 store 的 action：`cartStore.addToCart(product, quantity)`。

3.  **Store 邏輯 (`stores/cart.js`)**:
    - `addToCart` action 接收到商品和數量。
    - 它會檢查購物車中是否已存在該商品：
      - 如果是，則增加數量。
      - 如果否，則將新商品加入 `cartItems` 陣列。
    - Pinia 的 state 是透過 `reactive` 實現的，所以這個修改會被自動追蹤。

4.  **UI 自動更新**:
    - `CartSidebar.vue` 和導覽列上的購物車圖示都使用了 `useCartStore()` 來獲取購物車數據 (例如 `cartStore.cartItems` 或 `cartStore.totalItems` getter)。
    - 當 `cart.js` 的 state 改變時，所有使用到這些數據的組件都會 **自動重新渲染**，無需手動操作 DOM。

---

## 5. 面試 Demo 重點

- **展示專案結構**: 介紹 `src` 下各資料夾的職責，強調 `views`, `components`, `stores` 的分工，這能體現您組織大型前端專案的能力。
- **從 `main.js` 開始**: 解釋它是如何啟動整個應用的。
- **追蹤一個核心功能**:
  - **登入流程**: 從 `AuthModals.vue` 的表單提交 -> `auth.js` store 的 `login` action -> `api.js` 發送請求 -> 成功後儲存 token 並更新 `api.js` 的 header。解釋 JWT 如何在前端被儲存和使用。
  - **購物車功能**: 從點擊「加入購物車」 -> `cart.js` store 的 state 變化 -> 所有相關 UI (如側邊欄、導覽列角標) 自動更新。這能完美展示 Pinia 的強大之處。
- **解釋路由**: 打開 `router/index.js`，說明路徑和組件的對應關係。點擊幾個 `<router-link>`，展示 SPA 無刷新跳轉的特性。
- **強調 Composition API (`<script setup>`)**: 說明這是 Vue 3 的新特性，它如何讓相關邏輯（響應式狀態、方法、生命週期鉤子）組織得更集中，提高了程式碼的可讀性和可維護性。

---

祝您面試順利！
