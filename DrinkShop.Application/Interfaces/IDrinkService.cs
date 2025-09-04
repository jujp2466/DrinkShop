using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IDrinkService
    {
        Task<IEnumerable<DrinkDto>> GetAllAsync();
        Task<DrinkDto?> GetByIdAsync(int id);
    }
}

