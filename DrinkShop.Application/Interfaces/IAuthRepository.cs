using DrinkShop.Application.DTOs;

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
        // 更新使用者最後登入時間（以 UTC 儲存）
        Task UpdateLastLoginAsync(int userId, DateTime atUtc);
        // ...可依需求擴充
    }
}
