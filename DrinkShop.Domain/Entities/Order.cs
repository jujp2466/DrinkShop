using System;
using System.Collections.Generic;

namespace DrinkShop.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();
    }
}
