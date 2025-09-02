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
    public class AuthController : ControllerBase
    {
        private readonly DrinkShopDbContext _db;
        public AuthController(DrinkShopDbContext db)
        {
            _db = db;
        }

        public class RegisterDto {
            public string UserName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Address { get; set; }
            public string? Phone { get; set; }
            public bool IsActive { get; set; } = true;
        }
        public class LoginDto { public string UserName { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.UserName == dto.UserName || u.Email == dto.Email))
                return BadRequest(new { code = 400, message = "用戶名或郵箱已存在" });

            var user = new User {
                UserName = dto.UserName,
                Password = dto.Password,
                Email = dto.Email,
                Role = "user",
                Address = dto.Address,
                Phone = dto.Phone ?? string.Empty,
                IsActive = dto.IsActive,
                Status = dto.IsActive ? "active" : "inactive",
                LastLoginAt = null
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Registered", data = new { user.Id, user.UserName, user.Role, user.Address, user.Phone, user.IsActive, user.Status } });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName && u.Password == dto.Password);
            if (user == null) return Unauthorized(new { code = 401, message = "用戶名或密碼錯誤" });
            // 登入成功時更新最後登入時間
            user.LastLoginAt = DateTime.UtcNow;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            // 回傳所有欄位
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
