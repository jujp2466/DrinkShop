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
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
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
    }
}

