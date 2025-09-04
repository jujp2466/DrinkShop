using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                Email = u.Email,
                Role = u.Role,
                Phone = u.Phone,
                IsActive = u.IsActive,
                Address = u.Address,
                Status = u.Status,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt
            }).ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return new UserDto { 
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
