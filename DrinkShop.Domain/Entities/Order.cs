namespace DrinkShop.Domain.Entities
{
    /// <summary>
    /// 訂單資料表 Entity
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 主鍵 Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用戶 Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用戶導航屬性
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal ShippingFee { get; set; } = 0;

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public string Status { get; set; } = "pending";

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 收件地址
        /// </summary>
        public string? ShippingAddress { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// 訂單項目集合
        /// </summary>
        public List<OrderItem> Items { get; set; } = new();
    }
}
