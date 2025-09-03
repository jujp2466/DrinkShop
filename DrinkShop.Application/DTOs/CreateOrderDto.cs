using System.Collections.Generic;

namespace DrinkShop.Application.DTOs
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public List<CreateOrderItemDto> Items { get; set; } = new();
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
