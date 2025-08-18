using Microsoft.AspNetCore.Mvc;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using System.Collections.Generic;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DrinkController : ControllerBase
    {
        private readonly IDrinkService _service;
        public DrinkController(IDrinkService service)
        {
            _service = service;
        }

        /// <summary>
        /// 取得所有飲品的清單。
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var drinks = await _service.GetAllAsync();
            return Ok(new { code = 200, message = "Success", data = drinks });
        }

        /// <summary>
        /// 根據 ID 取得特定飲品的詳細資訊。
        /// </summary>
        /// <param name="id">飲品的唯一識別碼。</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var drink = await _service.GetByIdAsync(id);
            if (drink == null)
                return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Success", data = drink });
        }

        /// <summary>
        /// 新增一個新的飲品。
        /// </summary>
        /// <param name="drink">要新增的飲品物件。</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Drink drink)
        {
            // 確保接收到的 Drink 包含 Quantity
            await _service.AddAsync(drink);
            return Ok(new { code = 200, message = "Created", data = drink });
        }

        /// <summary>
        /// 根據 ID 刪除特定飲品。
        /// </summary>
        /// <param name="id">飲品的唯一識別碼。</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { code = 200, message = "Deleted" });
        }

        /// <summary>
        /// 處理結帳邏輯，並提交訂單。
        /// </summary>
        /// <param name="items">購物車中的飲品清單。</param>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] List<Drink> items)
        {
            if (items == null || items.Count == 0)
            {
                return BadRequest(new { message = "購物車為空，無法結帳。" });
            }

            // 提取飲品 ID
            var drinkIds = items.Select(item => item.Id).ToList();

            // 調用 PlaceOrder 方法處理訂單邏輯
            var drinks = await _service.GetDrinksByIdsAsync(drinkIds);

            if (drinks == null || !drinks.Any())
            {
                return NotFound(new { code = 404, message = "找不到對應的飲品" });
            }

            foreach (var drink in drinks)
            {
                drink.Quantity -= 1; // 假設每次訂購減少 1
            }

            await _service.UpdateDrinksAsync(drinks);

            // 計算總金額
            decimal totalAmount = items.Sum(item => item.Price);

            // 返回成功訊息
            return Ok(new { message = "結帳成功！", totalAmount, data = drinks });
        }
    }
}
