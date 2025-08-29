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
            public int? UserId { get; set; }
            public decimal TotalAmount { get; set; }
            public List<CreateOrderItemDto> Items { get; set; } = new();
        }

        // POST /api/v1/orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                TotalAmount = dto.TotalAmount,
                Status = "pending"
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
            IQueryable<Order> q = _db.Orders.OrderByDescending(o => o.Id);
            if (role != "admin")
            {
                if (userId == null) return BadRequest(new { code = 400, message = "Missing userId" });
                q = q.Where(o => o.UserId == userId);
            }
            var list = await q.ToListAsync();
            return Ok(new { code = 200, message = "Success", data = list });
        }
    }
}
