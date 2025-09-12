using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrinkShop.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrdersController(IOrderService service)
        {
            _service = service;
        }


            /// <summary>
            /// 新增訂單
            /// </summary>
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
            {
                var order = await _service.CreateAsync(dto);
                return Ok(new { code = 200, message = "Created", data = new { orderId = order.Id } });
            }


        /// <summary>
        /// 取得訂單列表（可依使用者或角色篩選）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? userId, [FromQuery] string? role)
        {
            var orders = await _service.GetAsync(userId, role);
            return Ok(new { code = 200, message = "Success", data = orders });
        }


        /// <summary>
        /// 更新訂單狀態
        /// </summary>
        [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderDto updated)
        {
            var order = await _service.UpdateStatusAsync(id, updated.Status);
            if (order == null) return NotFound(new { code = 404, message = "Order not found" });
            return Ok(new { code = 200, message = "Updated", data = new { order.Id } });
        }


        /// <summary>
        /// 刪除訂單
        /// </summary>
        [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { code = 404, message = "Order not found" });
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
