using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        // POST /api/v1/orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var order = await _service.CreateAsync(dto);
            return Ok(new { code = 200, message = "Created", data = new { orderId = order.Id } });
        }

        // GET /api/v1/orders?userId=123&role=admin|user
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? userId, [FromQuery] string? role)
        {
            var orders = await _service.GetAsync(userId, role);
            return Ok(new { code = 200, message = "Success", data = orders });
        }

        // PUT /api/v1/orders/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderDto updated)
        {
            var order = await _service.UpdateStatusAsync(id, updated.Status);
            if (order == null) return NotFound(new { code = 404, message = "Order not found" });
            return Ok(new { code = 200, message = "Updated", data = new { order.Id } });
        }

        // DELETE /api/v1/orders/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { code = 404, message = "Order not found" });
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
