using DrinkShop.Application.DTOs;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto?> RegisterAsync(RegisterDto dto);
        Task<UserDto?> LoginAsync(LoginDto dto);
    }
}
