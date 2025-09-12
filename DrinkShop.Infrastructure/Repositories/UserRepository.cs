using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Infrastructure.Repositories
{
    /// <summary>
    /// 使用者相關資料存取實作
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DrinkShopDbContext _context;
        public UserRepository(DrinkShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users.Select(u => new UserDto { 
                Id = u.Id, 
                UserName = u.UserName, 
                DisplayName = string.IsNullOrWhiteSpace(u.UserName) ? (string.IsNullOrWhiteSpace(u.Email) ? (u.Address ?? "") : u.Email) : u.UserName,
                Email = u.Email,
                Role = u.Role,
                Phone = u.Phone,
                IsActive = u.IsActive,
                Address = u.Address,
                Status = u.Status,
                LastLoginAt = u.LastLoginAt.HasValue ? DateTime.SpecifyKind(u.LastLoginAt.Value, DateTimeKind.Utc) : (DateTime?)null,
                CreatedAt = DateTime.SpecifyKind(u.CreatedAt, DateTimeKind.Utc)
            }).ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return new UserDto { 
                Id = user.Id, 
                UserName = user.UserName, 
                DisplayName = string.IsNullOrWhiteSpace(user.UserName) ? (string.IsNullOrWhiteSpace(user.Email) ? (user.Address ?? "") : user.Email) : user.UserName,
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
    }
}
