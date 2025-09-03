using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using DrinkShop.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly DrinkShopDbContext _db;
        public OrderService(DrinkShopDbContext db)
        {
            _db = db;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                TotalAmount = dto.TotalAmount,
                ShippingFee = dto.ShippingFee,
                Status = "pending",
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            foreach (var item in dto.Items)
            {
                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }
            await _db.SaveChangesAsync();
            return await GetOrderDtoAsync(order.Id);
        }

        public async Task<IEnumerable<OrderDto>> GetAsync(int? userId, string? role)
        {
            IQueryable<Order> q = _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.Id);
            if (role != "admin")
            {
                if (userId == null) return new List<OrderDto>();
                q = q.Where(o => o.UserId == userId);
            }
            var orders = await q.ToListAsync();
            return orders.Select(o => MapOrderDto(o));
        }

        public async Task<OrderDto?> UpdateStatusAsync(int id, string status)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return null;
            order.Status = status ?? order.Status;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return await GetOrderDtoAsync(order.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return false;
            var items = _db.OrderItems.Where(i => i.OrderId == order.Id);
            _db.OrderItems.RemoveRange(items);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return true;
        }

        private async Task<OrderDto?> GetOrderDtoAsync(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            return order == null ? null : MapOrderDto(order);
        }

        private OrderDto MapOrderDto(Order o)
        {
            return new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                User = o.User == null ? null : new UserDto
                {
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
                ShippingFee = o.ShippingFee,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                PaymentMethod = o.PaymentMethod,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                Items = o.Items?.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? $"產品 {i.ProductId}",
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}
