using System.ComponentModel.DataAnnotations.Schema;

namespace DrinkShop.Domain.Entities
{
    public class Drink
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名稱
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
        /// 用於前端傳遞的臨時數據
        /// </summary>
        [NotMapped]
        public int Quantity { get; set; }
    }
}
