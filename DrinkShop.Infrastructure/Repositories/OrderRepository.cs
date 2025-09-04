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
    /// 訂單相關資料存取實作
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly DrinkShopDbContext _context;
        public OrderRepository(DrinkShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .Select(o => new OrderDto { 
                    Id = o.Id, 
                    UserId = o.UserId, 
                    // map User info when available
                    User = o.User == null ? null : new UserDto {
                        Id = o.User.Id,
                        UserName = o.User.UserName,
                        Email = o.User.Email,
                        Role = o.User.Role,
                        Address = o.User.Address,
                        Phone = o.User.Phone,
                        IsActive = o.User.IsActive,
                        Status = o.User.Status,
                        LastLoginAt = o.User.LastLoginAt,
                        CreatedAt = o.User.CreatedAt
                    },
                    TotalAmount = o.TotalAmount, 
                    Status = o.Status, 
                    CreatedAt = o.CreatedAt,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    User = o.User == null ? null : new UserDto {
                        Id = o.User.Id,
                        UserName = o.User.UserName,
                        Email = o.User.Email,
                        Role = o.User.Role,
                        Address = o.User.Address,
                        Phone = o.User.Phone,
                        IsActive = o.User.IsActive,
                        Status = o.User.Status,
                        LastLoginAt = o.User.LastLoginAt,
                        CreatedAt = o.User.CreatedAt
                    },
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderDto { 
                Id = order.Id, 
                UserId = order.UserId, 
                User = order.User == null ? null : new UserDto {
                    Id = order.User.Id,
                    UserName = order.User.UserName,
                    Email = order.User.Email,
                    Role = order.User.Role,
                    Address = order.User.Address,
                    Phone = order.User.Phone,
                    IsActive = order.User.IsActive,
                    Status = order.User.Status,
                    LastLoginAt = order.User.LastLoginAt,
                    CreatedAt = order.User.CreatedAt
                },
                TotalAmount = order.TotalAmount, 
                Status = order.Status, 
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }

        public async Task<bool> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var entity = new Order 
            { 
                UserId = createOrderDto.UserId, 
                TotalAmount = createOrderDto.TotalAmount,
                ShippingAddress = createOrderDto.ShippingAddress,
                PaymentMethod = createOrderDto.PaymentMethod,
                Notes = createOrderDto.Notes,
                ShippingFee = createOrderDto.ShippingFee,
                Items = createOrderDto.Items.Select(i => new OrderItem {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
            _context.Orders.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
