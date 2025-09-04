using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }


            /// <summary>
            /// 使用者註冊
            /// </summary>
            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] RegisterDto dto)
            {
                var user = await _service.RegisterAsync(dto);
                if (user == null)
                    return BadRequest(new { code = 400, message = "用戶名或郵箱已存在" });
                return Ok(new { code = 200, message = "Registered", data = new { user.Id, user.UserName, user.Role, user.Address, user.Phone, user.IsActive, user.Status } });
            }

        /// <summary>
        /// 使用者登入
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _service.LoginAsync(dto);
            if (user == null) return Unauthorized(new { code = 401, message = "用戶名或密碼錯誤" });
            return Ok(new {
                code = 200,
                message = "Success",
                data = new {
                    user.Id,
                    user.UserName,
                    user.Role,
                    user.Address,
                    user.Phone,
                    user.IsActive,
                    user.Status,
                    user.LastLoginAt
                }
            });
        }
    }
}
