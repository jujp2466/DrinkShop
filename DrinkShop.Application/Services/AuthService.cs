using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using System.Threading.Tasks;

namespace DrinkShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

    /// <summary>
    /// 使用者註冊
    /// </summary>
        public async Task<UserDto?> RegisterAsync(RegisterDto dto)
        {
            // Check if user already exists
            var existingUser = await _authRepository.GetUserByAccountAsync(dto.UserName);
            if (existingUser != null)
            {
                // Or throw a custom exception
                return null; 
            }
            var success = await _authRepository.RegisterUserAsync(dto);
            if (!success) return null;
            return await _authRepository.GetUserByAccountAsync(dto.UserName);
        }

    /// <summary>
    /// 使用者登入
    /// </summary>
        public async Task<UserDto?> LoginAsync(LoginDto dto)
        {
            // The repository is now responsible for password validation
            var user = await _authRepository.ValidateUserAsync(dto.UserName, dto.Password);
            if (user == null) return null;
            
            // TODO: Implement logic to update LastLoginAt in the repository
            return user;
        }
    }
}
