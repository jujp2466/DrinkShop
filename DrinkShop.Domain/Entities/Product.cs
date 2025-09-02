using System;

namespace DrinkShop.Domain.Entities
{
    /// <summary>
    /// 產品資料表 Entity
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 主鍵 Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 產品描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        [System.ComponentModel.Description("分類")]
        public string? Category { get; set; }

        /// <summary>
        /// 圖片網址
        /// </summary>
        [System.ComponentModel.Description("圖片網址")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 庫存
        /// </summary>
        [System.ComponentModel.Description("庫存")]
        public int Stock { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 建立時間
        /// </summary>
        [System.ComponentModel.Description("建立時間")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
