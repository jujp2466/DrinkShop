using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using DrinkShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DrinkShop.Infrastructure.Repositories
{
    /// <summary>
    /// 驗證相關資料存取實作
    /// </summary>
    public class AuthRepository : IAuthRepository
    {
        private readonly DrinkShopDbContext _context;
        public AuthRepository(DrinkShopDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetUserByAccountAsync(string username)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                IsActive = user.IsActive,
                Address = user.Address,
                Status = user.Status,
                LastLoginAt = user.LastLoginAt.HasValue ? DateTime.SpecifyKind(user.LastLoginAt.Value, DateTimeKind.Utc) : (DateTime?)null,
                CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc)
            };
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            var entity = new User
            {
                UserName = registerDto.UserName,
                // In a real app, you should hash the password
                Password = registerDto.Password,
                Email = registerDto.Email,
                IsActive = true,
                Role = "User", // Default role
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UserDto?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                IsActive = user.IsActive,
                Address = user.Address,
                Status = user.Status,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }

        // 更新使用者最後登入時間，將值寫回 DB（以 UTC 儲存）
        public async Task UpdateLastLoginAsync(int userId, DateTime atUtc)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;
            user.LastLoginAt = DateTime.SpecifyKind(atUtc, DateTimeKind.Utc);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}

