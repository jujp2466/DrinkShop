using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    /// <summary>
    /// 用於存取使用者相關資料的介面
    /// </summary>
    public interface IUserRepository
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        // ...可依需求擴充
    }
}
