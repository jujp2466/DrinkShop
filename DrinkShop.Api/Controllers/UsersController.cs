using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }


            /// <summary>
            /// 取得所有使用者
            /// </summary>
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var result = await _service.GetAllAsync();
                return Ok(new { code = 200, message = "Success", data = result });
            }


        /// <summary>
        /// 更新使用者資料
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto update)
        {
            var user = await _service.UpdateAsync(id, update);
            if (user == null) return NotFound(new { code = 404, message = "User not found" });
            return Ok(new { code = 200, message = "Updated", data = new { user.Id } });
        }


        /// <summary>
        /// 刪除使用者
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { code = 404, message = "User not found" });
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
