# 後端專案架構與技術解析 (PROJECT_OVERVIEW.md)

這份文件旨在協助您在面試時，清晰地介紹此 .NET 後端專案的架構、技術選型與核心邏輯。

---

## 1. 專案概覽

本專案是一個飲料店的後端 API 服務，採用 **.NET 7** 和 **Clean Architecture (清晰架構)** 設計模式，旨在實現高內聚、低耦合、易於維護和擴展的目標。

 **主要技術棧**:
  - **框架**: ASP.NET Core 9
  - **資料庫**: Entity Framework Core 9 + SQLite (易於開發和部署)
  - **架構**: Clean Architecture
  - **驗證**: JWT (JSON Web Tokens)

---

## 2. Clean Architecture (清晰架構)

這是本專案最大的亮點，您可以強調您對軟體架構的理解。

**核心理念**: 依賴關係由外向內，確保核心業務邏輯 (Domain) 的獨立性。

![Clean Architecture Diagram](https://i.imgur.com/9Nab4u3.png)

專案結構對應如下：

- `DrinkShop.Domain` (**核心層 - Entities**):
  - **用途**: 定義專案最核心的業務實體 (Entities)，例如 `Product`, `Order`, `User`。
  - **特點**: **不依賴任何其他層**。這是整個應用程式的心臟，只包含純粹的業務模型和規則。

- `DrinkShop.Application` (**應用層 - Use Cases**):
  - **用途**: 實現應用程式的業務邏輯。
    - **Interfaces**: 定義各種服務和倉儲的介面，如 `IProductService`, `IOrderRepository`。
    - **Services**: 實現具體的業務流程，例如 `OrderService` 處理訂單創建的邏輯。
    - **DTOs (Data Transfer Objects)**: 定義與外部 (API層) 溝通的資料結構，如 `CreateOrderDto`。
  - **依賴**: 只依賴 `Domain` 層。

- `DrinkShop.Infrastructure` (**基礎設施層 - Frameworks & Drivers**):
  - **用途**: 處理所有與外部技術相關的實作。
    - **`DrinkShopDbContext.cs`**: Entity Framework Core 的資料庫上下文，負責與資料庫溝通。
    - **Repositories**: 實作 `Application` 層定義的倉儲介面，例如 `ProductRepository` 負責用 EF Core 存取商品資料。
    - **Migrations**: 資料庫結構的變更紀錄。
  - **依賴**: 依賴 `Application` 層。

- `DrinkShop.Api` (**表現層 - Interface Adapters**):
  - **用途**: 作為專案的進入點，處理 HTTP 請求和回應。
    - **`Program.cs`**: 應用程式的啟動設定檔。
    - **Controllers**: 接收 HTTP 請求，呼叫 `Application` 層的服務來處理，並回傳 DTO。
    - **Middlewares**: 處理跨領域的關注點，例如錯誤處理、身分驗證。
  - **依賴**: 依賴 `Application` 層。

---

## 3. 程式進入點與請求生命週期

您可以透過一個請求的完整流程來展示您對專案的掌握度。

**範例：一個「建立訂單」的請求 (`POST /api/orders`)**

1.  **進入點 (`Program.cs`)**:
    - 應用程式啟動時，會在這裡設定所有服務的 **依賴注入 (Dependency Injection, DI)**。例如，將 `IOrderService` 介面與 `OrderService` 實作綁定。
    - 同時設定中介軟體 (Middleware) 管道，如 `app.UseCors()`, `app.UseAuthentication()`, `app.MapControllers()`。

2.  **API 層 (`OrdersController.cs`)**:
    - `[HttpPost]` Action 接收到 HTTP 請求，請求的 Body 會被模型綁定到 `CreateOrderDto` 物件。
    - Controller **透過 DI 注入 `IOrderService`**。
    - 呼叫 `_orderService.CreateOrderAsync(orderDto)`。

3.  **應用層 (`OrderService.cs`)**:
    - 接收到 DTO 後，進行業務邏輯處理：
      - 驗證商品庫存 (如果需要)。
      - 計算總金額。
      - 將 `CreateOrderDto` 轉換為 `Order` 和 `OrderItem` 等 `Domain` 實體。
    - 呼叫 `_orderRepository.AddAsync(order)` 將訂單實體傳遞到倉儲層。

4.  **基礎設施層 (`OrderRepository.cs`)**:
    - 接收到 `Order` 實體。
    - 透過 `_dbContext.Orders.Add(order)`，使用 Entity Framework Core 將實體轉換為資料庫操作。
    - 呼叫 `_dbContext.SaveChangesAsync()` 將變更寫入 SQLite 資料庫。

5.  **回傳響應**:
    - 請求沿原路回傳，`OrderService` 可能會回傳一個包含訂單 ID 的 `OrderDto`。
    - `OrdersController` 最終將 `OrderDto` 序列化為 JSON 並回傳給前端。

---

## 4. 面試 Demo 重點

- **展示 Clean Architecture**: 解釋各專案 (Domain, Application, Infrastructure, Api) 的職責和依賴方向，這會讓面試官覺得你具備大型專案的架構能力。
- **解釋依賴注入**: 打開 `Program.cs`，說明如何註冊服務 (`builder.Services.AddScoped<IProductService, ProductService>()`)，並在 `ProductsController.cs` 中展示如何透過建構子注入使用。
- **追蹤一個 API**: 實際操作一次，從 Controller -> Service -> Repository -> DB，解釋 DTO 和 Entity 的轉換，以及各層的職責。
- **身分驗證 (JWT)**:
    - 說明登入 API (`AuthController`) 如何驗證使用者並產生 JWT。
    - 展示某個需要授權的 API (例如 `[Authorize]`)，並解釋請求 Header 中的 `Bearer Token` 是如何被驗證的。
- **資料庫與 EF Core**:
    - 打開 `DrinkShopDbContext.cs`，展示如何定義 `DbSet`。
    - 說明 `Migrations` 資料夾的作用，表示您了解資料庫版本控制。

---

祝您面試順利！
