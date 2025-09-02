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
            var result = list.Select(u => new { u.Id, u.UserName, u.Email, u.Role, u.CreatedAt });
            return Ok(new { code = 200, message = "Success", data = result });
        }

        // PUT /api/v1/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] User update)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound(new { code = 404, message = "User not found" });

            // only allow updating certain fields
            user.UserName = update.UserName ?? user.UserName;
            user.Email = update.Email ?? user.Email;
            user.Role = update.Role ?? user.Role;
            user.Phone = update.Phone ?? user.Phone;
            user.Address = update.Address ?? user.Address;
            user.Status = update.Status ?? user.Status;
            user.IsActive = update.IsActive;
            user.LastLoginAt = update.LastLoginAt ?? user.LastLoginAt;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Updated", data = new { user.Id } });
        }

        // DELETE /api/v1/users/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound(new { code = 404, message = "User not found" });
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
