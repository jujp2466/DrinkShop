using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DrinkShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
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
        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.ValidateUserAsync(dto.UserName, dto.Password);
            if (user == null) return null;

            var nowUtc = DateTime.UtcNow;
            await _authRepository.UpdateLastLoginAsync(user.Id, nowUtc);

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserDto user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"] ?? "7"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
