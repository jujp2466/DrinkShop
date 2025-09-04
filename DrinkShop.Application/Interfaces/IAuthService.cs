using DrinkShop.Application.DTOs;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IAuthService
    {
    /// <summary>
    /// 使用者註冊
    /// </summary>
    Task<UserDto?> RegisterAsync(RegisterDto dto);
    /// <summary>
    /// 使用者登入
    /// </summary>
    Task<UserDto?> LoginAsync(LoginDto dto);
    }
}
