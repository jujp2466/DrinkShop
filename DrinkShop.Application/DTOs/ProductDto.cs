namespace DrinkShop.Application.DTOs
{
    /// <summary>
    /// 商品 DTO（Data Transfer Object）
    /// 用於服務層與 API 控制器之間傳遞商品資料，避免直接暴露 Entity。
    /// </summary>
    public class ProductDto
    {
        // 商品識別編號（由資料庫自動產生）
        public int Id { get; set; }

        // 產品名稱
        public string Name { get; set; } = string.Empty;

        // 產品描述（可能為空字串）
        public string Description { get; set; } = string.Empty;

        // 價格（以新台幣或系統設定的貨幣為單位）
        public decimal Price { get; set; }

        // 分類名稱
        public string Category { get; set; } = string.Empty;

        // 圖片網址（可為空）
        public string ImageUrl { get; set; } = string.Empty;

        // 庫存數量
        public int Stock { get; set; }

        // 是否上架（true = 上架, false = 下架）
        public bool IsActive { get; set; }

        // 建立時間：由資料庫或 EF Core 在儲存時回寫
        // 注意：DTO 使用 DateTime 非 nullable，若有可能為 null 可改為 DateTime?
        public DateTime CreatedAt { get; set; }
    }
}
