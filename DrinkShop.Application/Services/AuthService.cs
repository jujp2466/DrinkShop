using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using DrinkShop.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly DrinkShopDbContext _db;
        public AuthService(DrinkShopDbContext db)
        {
            _db = db;
        }

        public async Task<UserDto?> RegisterAsync(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.UserName == dto.UserName || u.Email == dto.Email))
                return null;
            var user = new User
            {
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
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Address = user.Address,
                Phone = user.Phone,
                IsActive = user.IsActive,
                Status = user.Status,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto?> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName && u.Password == dto.Password);
            if (user == null) return null;
            user.LastLoginAt = DateTime.UtcNow;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Address = user.Address,
                Phone = user.Phone,
                IsActive = user.IsActive,
                Status = user.Status,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
