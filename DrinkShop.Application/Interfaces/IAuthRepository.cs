using DrinkShop.Application.DTOs;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    /// <summary>
    /// 用於存取使用者驗證相關資料的介面
    /// </summary>
    public interface IAuthRepository
    {
        Task<UserDto?> GetUserByAccountAsync(string account);
        Task<bool> RegisterUserAsync(RegisterDto registerDto);
        Task<UserDto?> ValidateUserAsync(string username, string password);
        // ...可依需求擴充
    }
}
