using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IUserService
    {
    /// <summary>
    /// 取得所有使用者
    /// </summary>
    Task<IEnumerable<UserDto>> GetAllAsync();
    /// <summary>
    /// 更新使用者資料
    /// </summary>
    Task<UserDto?> UpdateAsync(int id, UserDto update);
    /// <summary>
    /// 刪除使用者
    /// </summary>
    Task<bool> DeleteAsync(int id);
    }
}
