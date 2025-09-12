namespace DrinkShop.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        // 統一前端顯示的訂單顧客名稱（來自 User 或 order.customerName）
        public string DisplayName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public string Status { get; set; } = "pending";
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public int ItemCount => Items?.Count ?? 0;
    }
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
