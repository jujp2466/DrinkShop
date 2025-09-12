using DrinkShop.Application.DTOs;

namespace DrinkShop.Application.Interfaces
{
    public interface IAuthService
    {
    /// <summary>
    /// 使用者註冊
    /// </summary>
    Task<UserDto?> RegisterAsync(RegisterDto dto);
    /// <summary>
    /// 使用者登入，成功時回傳 JWT
    /// </summary>
    Task<string?> LoginAsync(LoginDto dto);
    }
}
