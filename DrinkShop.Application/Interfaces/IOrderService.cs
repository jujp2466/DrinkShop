using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<IEnumerable<OrderDto>> GetAsync(int? userId, string? role);
        Task<OrderDto?> UpdateStatusAsync(int id, string status);
        Task<bool> DeleteAsync(int id);
    }
}
