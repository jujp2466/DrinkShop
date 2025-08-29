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

        public class RegisterDto { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; }
        public class LoginDto { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return BadRequest(new { code = 400, message = "用戶名或郵箱已存在" });

            var user = new User { Username = dto.Username, Password = dto.Password, Email = dto.Email, Role = "user" };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Registered", data = new { user.Id, user.Username, user.Role } });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Password == dto.Password);
            if (user == null) return Unauthorized(new { code = 401, message = "用戶名或密碼錯誤" });
            // 簡化：直接回傳 user（可改 JWT）
            return Ok(new { code = 200, message = "Success", data = new { user.Id, user.Username, user.Role } });
        }
    }
}
