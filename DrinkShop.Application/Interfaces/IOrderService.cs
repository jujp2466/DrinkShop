using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IOrderService
    {
    /// <summary>
    /// 新增訂單
    /// </summary>
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    /// <summary>
    /// 取得訂單列表（可依使用者或角色篩選）
    /// </summary>
    Task<IEnumerable<OrderDto>> GetAsync(int? userId, string? role);
    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    Task<OrderDto?> UpdateStatusAsync(int id, string status);
    /// <summary>
    /// 刪除訂單
    /// </summary>
    Task<bool> DeleteAsync(int id);
    }
}
