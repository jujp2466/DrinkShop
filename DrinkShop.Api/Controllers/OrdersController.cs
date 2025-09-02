using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrinkShop.Domain.Entities;
using DrinkShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly DrinkShopDbContext _db;
        public OrdersController(DrinkShopDbContext db)
        {
            _db = db;
        }

        public class CreateOrderItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
        public class CreateOrderDto
        {
            public int UserId { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal ShippingFee { get; set; } = 0;
            public List<CreateOrderItemDto> Items { get; set; } = new();
            // Order specific information (not customer info)
            public string? ShippingAddress { get; set; }
            public string? PaymentMethod { get; set; }
            public string? Notes { get; set; }
        }

        // POST /api/v1/orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                TotalAmount = dto.TotalAmount,
                ShippingFee = dto.ShippingFee,
                Status = "pending",
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var item in dto.Items)
            {
                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Created", data = new { orderId = order.Id } });
        }

        // GET /api/v1/orders?userId=123&role=admin|user
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? userId, [FromQuery] string? role)
        {
            IQueryable<Order> q = _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)  // 包含產品資訊
                .OrderByDescending(o => o.Id);
                
            if (role != "admin")
            {
                if (userId == null) return BadRequest(new { code = 400, message = "Missing userId" });
                q = q.Where(o => o.UserId == userId);
            }
            
            var orders = await q.ToListAsync();
            
            // Map to response format with customer info from User
            var responseData = orders.Select(o => new
            {
                id = o.Id,
                userId = o.UserId,
                user = o.User != null ? new
                {
                    id = o.User.Id,
                    name = o.User.UserName,
                    email = o.User.Email,
                    phone = o.User.Phone
                } : null,
                totalAmount = o.TotalAmount,
                shippingFee = o.ShippingFee,
                status = o.Status,
                shippingAddress = o.ShippingAddress,
                paymentMethod = o.PaymentMethod,
                notes = o.Notes,
                createdAt = o.CreatedAt,
                items = o.Items?.Select(i => new
                {
                    id = i.Id,
                    productId = i.ProductId,
                    productName = i.Product?.Name ?? $"產品 {i.ProductId}",
                    quantity = i.Quantity,
                    price = i.Price
                }).ToList(),
                itemCount = o.Items?.Count ?? 0
            }).ToList();
            
            return Ok(new { code = 200, message = "Success", data = responseData });
        }

        // PUT /api/v1/orders/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] Order updated)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound(new { code = 404, message = "Order not found" });
            order.Status = updated.Status ?? order.Status;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Updated", data = new { order.Id } });
        }

        // DELETE /api/v1/orders/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound(new { code = 404, message = "Order not found" });
            // remove related items first
            var items = _db.OrderItems.Where(i => i.OrderId == order.Id);
            _db.OrderItems.RemoveRange(items);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
