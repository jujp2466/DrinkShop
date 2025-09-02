namespace DrinkShop.Domain.Entities
{
    /// <summary>
    /// 訂單項目資料表 Entity
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// 主鍵 Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 所屬訂單 Id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 產品 Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 單價
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 訂單導航屬性
        /// </summary>
        public Order Order { get; set; } = null!;

        /// <summary>
        /// 產品導航屬性
        /// </summary>
        public Product Product { get; set; } = null!;
    }
}
