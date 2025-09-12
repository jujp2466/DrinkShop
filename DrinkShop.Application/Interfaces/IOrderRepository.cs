using DrinkShop.Application.DTOs;

namespace DrinkShop.Application.Interfaces
{
    /// <summary>
    /// 用於存取訂單相關資料的介面
    /// </summary>
    public interface IOrderRepository
    {
        Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<bool> CreateOrderAsync(CreateOrderDto createOrderDto);
        // ...可依需求擴充
    }
}
