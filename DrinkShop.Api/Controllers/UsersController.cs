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
    public class UsersController : ControllerBase
    {
        private readonly DrinkShopDbContext _db;
        public UsersController(DrinkShopDbContext db)
        {
            _db = db;
        }

        // GET /api/v1/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Users.OrderByDescending(u => u.Id).ToListAsync();
            var result = list.Select(u => new { u.Id, u.Username, u.Email, u.Role, u.CreatedAt });
            return Ok(new { code = 200, message = "Success", data = result });
        }
    }
}
