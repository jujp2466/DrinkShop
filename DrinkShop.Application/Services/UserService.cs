using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using DrinkShop.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly DrinkShopDbContext _db;
        public UserService(DrinkShopDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var list = await _db.Users.OrderByDescending(u => u.Id).ToListAsync();
            return list.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role,
                Address = u.Address,
                Phone = u.Phone,
                IsActive = u.IsActive,
                Status = u.Status,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserDto?> UpdateAsync(int id, UserDto update)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return null;
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

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return false;
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
