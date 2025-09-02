using System.ComponentModel.DataAnnotations.Schema;

namespace DrinkShop.Domain.Entities
{
    /// <summary>
    /// 飲品資料表 Entity
    /// </summary>
    public class Drink
    {
        /// <summary>
        /// 主鍵 Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 飲品名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 商品被購買的總次數（累計數量）
        /// </summary>
        public int PurchaseCount { get; set; } = 0;

        /// <summary>
        /// 用於前端傳遞的臨時數據
        /// </summary>
    [NotMapped]
        public int Quantity { get; set; }
    }
}
