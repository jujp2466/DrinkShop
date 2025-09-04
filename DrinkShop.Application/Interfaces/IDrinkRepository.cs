using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IDrinkRepository
    {
        Task<List<DrinkDto>> GetAllDrinksAsync();
        Task<DrinkDto?> GetDrinkByIdAsync(int id);
    }
}

