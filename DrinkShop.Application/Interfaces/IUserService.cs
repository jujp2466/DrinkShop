using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> UpdateAsync(int id, UserDto update);
        Task<bool> DeleteAsync(int id);
    }
}
