using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

    /// <summary>
    /// 取得所有使用者
    /// </summary>
    public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

    /// <summary>
    /// 更新使用者資料
    /// </summary>
    public async Task<UserDto?> UpdateAsync(int id, UserDto update)
        {
            // TODO: Implement update logic in repository
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;
            
            // Example of what might be in the repository's update method
            user.UserName = update.UserName;
            user.Email = update.Email;
            // await _userRepository.UpdateAsync(user);

            return user;
        }

    /// <summary>
    /// 刪除使用者
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
        {
            // TODO: Implement delete logic in repository
            // return await _userRepository.DeleteAsync(id);
            return await Task.FromResult(false);
        }
    }
}
